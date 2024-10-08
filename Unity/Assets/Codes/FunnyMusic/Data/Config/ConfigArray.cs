using System.Collections.Generic;
using System.Text;

namespace FunnyMusic
{

    public struct ConfigArray<T>
    {
        public T[] _array;

        public int Length
        {
            get
            {
                return _array == null ? 0 : _array.Length;
            }
        }

        public ConfigArray(T[] array)
        {
            _array = array;
        }

        public void CopyTo(T[] array, int index)
        {
            for(int i = index; i < Length; i++)
            {
                array[i - index] = _array[i];
            }
        }

        public T this[int index]
        {
            get
            {
                if (_array == null || index >= _array.Length)
                {
                    return default(T);
                }
                return _array[index];
            }
            set
            {
                _array[index] = value;
            }
        }

        public static implicit operator ConfigArray<T>(T[] m)
        {
            return new ConfigArray<T>(m);
        }

        public List<T> ConvertToList()
        {
            List<T> result = null;
            if (null != _array)
            {
                result = new List<T>(_array.Length);
                for (int index = 0; index < _array.Length; ++index)
                {
                    result.Add(_array[index]);
                }
            }
            else
            {
                result = new List<T>();
            }

            return result;
        }



        public bool Contains(T item)
        {
            if (null != _array)
            {
                if ((System.Object)item == null)
                {
                    for (int index = 0; index < _array.Length; ++index)
                    {
                        if (null == (System.Object)_array[index])
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    EqualityComparer<T> comparer = EqualityComparer<T>.Default;
                    for (int index = 0; index < _array.Length; ++index)
                    {
                        if (comparer.Equals(_array[index], item))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _array.Length; i++)
            {
                sb.Append($"{_array[i].ToString()} ");
            }

            return sb.ToString();
        }
    }

    public struct ConfigArrayArray<T>
    {
        private static ConfigArray<T> _empty = new ConfigArray<T>();

        public ConfigArray<T>[] _array;

        public int Length
        {
            get
            {
                return _array == null ? 0 : _array.Length;
            }
        }

        public ConfigArrayArray(ConfigArray<T>[] array)
        {
            _array = array;
        }

        public ConfigArrayArray(T[][] array)
        {
            if (array != null)
            {
                _array = new ConfigArray<T>[array.Length];
                for (var i = 0; i < array.Length; ++i)
                {
                    _array[i] = new ConfigArray<T>(array[i]);
                }
            }
            else
            {
                _array = null;
            }
        }

        public ConfigArray<T> this[int index]
        {
            get
            {
                if (_array == null)
                {
                    return _empty;
                }
                if (index >= _array.Length)
                {
                    return _array[0];
                }
                return _array[index];
            }
        }

        public static implicit operator ConfigArrayArray<T>(T[][] m)
        {
            return new ConfigArrayArray<T>(m);
        }
    }

}