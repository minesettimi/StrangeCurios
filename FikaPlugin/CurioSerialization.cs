using CuriosClient.Models;
using Fika.Core.Networking.LiteNetLib.Utils;

namespace StrangeCuriosFika;

public static class CurioSerialization
{
    public static CurioComponentDescriptor ReadCurioDescriptor(this NetDataReader reader)
    {
        return new CurioComponentDescriptor
        {
            NumberOfUsages = reader.GetInt()
        };
    }
    
    public static void PutCurioDescriptor(this NetDataWriter writer, CurioComponentDescriptor descriptor)
    {
        writer.Put(descriptor.NumberOfUsages);
    }
}