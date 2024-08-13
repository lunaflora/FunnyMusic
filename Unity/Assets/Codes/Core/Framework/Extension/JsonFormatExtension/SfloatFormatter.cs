using System;
using Framework;


namespace CatJson
{
    public class SfloatFormatter : BaseJsonFormatter<sfloat>
    {
        public override void ToJson(JsonParser parse, sfloat value, Type type, Type realType, int depth)
        {
            parse.Append(((float)value * GlobalMagicNumber.ConfigFixNumberScale).ToString());
          
        }

        /// <inheritdoc />
        public override sfloat ParseJson(JsonParser parser,Type type, Type realType)
        {
            long x = parser.Lexer.GetNextTokenByType(TokenType.Number).AsLong();

            return (sfloat)((float)x / (float)GlobalMagicNumber.ConfigFixNumberScale);
        }

    }


}