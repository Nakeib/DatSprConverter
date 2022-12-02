using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTibiaUnity.Protobuf.Appearances;

namespace OpenTibiaUnity.Core.Converter
{
    public class SpriteTypeImpl
    {
        public string File;
        public int SpriteType;
        public uint FirstSpriteID;
        public uint LastSpriteID;
        public uint AtlasID;
    }

    interface IConverter
    {
        Task<bool> BeginProcessing();
        bool BeginMapping();

        List<SpriteTypeImpl> GetSpriteSheet();
    }
}
