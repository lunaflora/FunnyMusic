//==================={By Qcbf|qcbf@qq.com|10/8/2021 7:17:45 PM}===================

using FLib;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace FLib
{
    public class LogWriter
    {
        public const byte LINE_ENDINGS = 10; // '\n'
        public const int PRINT_BUFFER_SIZE = 20480;
        public const int ERROR_BUFFER_SIZE = 4096;
        public const int MAX_FILE_SIZE = 1024 * 1024 * 30;
        public string DirectoryPath;

        private FileStream mErrorStream;
        private FileStream mPrintStream;
        private readonly ConcurrentQueue<LogData> mLogBuffer = new();
        private byte[] mWriteBuffer = new byte[1024];


        private struct LogData
        {
            public int Day;
            public FDebug.EType Type;
            public string Text;
        }


        public LogWriter(string name, string baseDirectory = "", bool catchUnhandledException = true)
        {
            try
            {
                DirectoryPath = Path.Combine(baseDirectory, name?.ToLowerInvariant() ?? "logs");
                FIO.CreateDirectory(DirectoryPath);
                FDebug.OnOutputEvent += OnLogOutputEvent;
                if (catchUnhandledException)
                {
                    AppDomain.CurrentDomain.ProcessExit += OnExit;
                    AppDomain.CurrentDomain.UnhandledException += OnException;
                }

                Task.Run(() =>
                {
                    var lastFlushTime = Environment.TickCount;
                    while (mWriteBuffer != null)
                    {
                        try
                        {
                            if (!mLogBuffer.IsEmpty)
                            {
                                Flush(false);
                                var t = Environment.TickCount; 
                                if (t - lastFlushTime > 5000)
                                {
                                    lastFlushTime = t;
                                    mPrintStream?.Flush();
                                    mErrorStream?.Flush();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            FDebug.Error(ex);
                        }

                        Thread.Sleep(10);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                FDebug.Error(ex.ToString());
            }
        }

        private static void OnExit(object sender, EventArgs e)
        {
            Console.WriteLine(FDebug.GetNowDate() + "Process Exited");
            FDebug.Print("Process Exited");
        }

        private static void OnException(object sender, UnhandledExceptionEventArgs e)
        {
            var log = "UnhandledException: " + sender + "\n" + e.ExceptionObject;
            Console.WriteLine(FDebug.GetNowDate() + log);
            FDebug.Error(log);
        }

        private void OnLogOutputEvent(FDebug.EType type, string text)
        {
            mLogBuffer.Enqueue(new LogData { Day = DateTime.Now.Day, Type = type, Text = text });
        }


        /// <summary>
        /// 
        /// </summary>
        public void Flush(bool isImmediateWriteToDisk)
        {
            var day = DateTime.Now.Day;
            foreach (var item in mLogBuffer)
            {
                var count = StringFLibUtility.Encoding.GetByteCount(item.Text) + 2;
                if (mWriteBuffer.Length < count)
                    mWriteBuffer = new byte[MathEx.GetNextPowerOfTwo(count)];
                StringFLibUtility.Encoding.GetBytes(item.Text, 0, item.Text.Length, mWriteBuffer, 0);
                mWriteBuffer[count - 2] = LINE_ENDINGS;
                mWriteBuffer[count - 1] = LINE_ENDINGS;

                if (item.Type == FDebug.EType.Error)
                {
                    if (item.Day != day || mErrorStream == null || mErrorStream.Length > MAX_FILE_SIZE || !File.Exists(mErrorStream.Name))
                        CheckStream(ref mErrorStream, "err", ERROR_BUFFER_SIZE, true);
                    mErrorStream.Write(mWriteBuffer, 0, count);
                }
                else
                {
                    if (item.Day != day || mPrintStream == null || mPrintStream.Length > MAX_FILE_SIZE || !File.Exists(mPrintStream.Name))
                        CheckStream(ref mPrintStream, "log", PRINT_BUFFER_SIZE, true);
                    mPrintStream.Write(mWriteBuffer, 0, count);
                }
            }

            mLogBuffer.Clear();
            if (isImmediateWriteToDisk)
            {
                mErrorStream?.Flush(true);
                mPrintStream?.Flush(true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CheckStream(ref FileStream stream, string type, int bufferSize, bool isForceReopen)
        {
            if (isForceReopen || stream == null || !stream.CanWrite || stream.Length > MAX_FILE_SIZE)
            {
                var strbuf = StringFLibUtility.GetStrBuf()
                    .Append(DateToFileName(DateTime.Now))
                    .Append('-')
                    .Append(type)
                    .Append("-00")
                    .Append(".log");
                var path = Path.Combine(DirectoryPath, StringFLibUtility.ReleaseStrBufAndResult(strbuf));
                var fi = new FileInfo(path);
                if (fi.Exists && fi.Length >= MAX_FILE_SIZE)
                {
                    path = path[..^7] + ".log";
                    path = FIO.SafePath(true, path, "-", 2);
                }

                stream?.Close();
                stream = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, bufferSize, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string DateToFileName(in DateTime d)
        {
            return d.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            FDebug.Print("Close LogWriter");
            FDebug.OnOutputEvent -= OnLogOutputEvent;
            AppDomain.CurrentDomain.ProcessExit -= OnExit;
            AppDomain.CurrentDomain.UnhandledException -= OnException;
            Flush(true);
            mWriteBuffer = null;
        }
    }
}
