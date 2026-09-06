using CuriosClient.Models;
using EFT.BinarySerialization;
using Mirror;

namespace CuriosClient.Extensions;

public static class DescriptorExtensions
{
    public static CurioComponentDescriptor? Clone(this CurioComponentDescriptor? source)
    {
        return source == null ? null : CloneCurioComponentDescriptor(source);
    }

    public static CurioComponentDescriptor CloneCurioComponentDescriptor(CurioComponentDescriptor source)
    {
        return new CurioComponentDescriptor
        {
            UsesLeft = BinaryCloneExtensions.CloneStruct(source.UsesLeft)
        };
    }

    public static CurioComponentDescriptor ReadCurioComponentDescriptor(this NetworkReader reader)
    {
        return new CurioComponentDescriptor()
        {
            UsesLeft = reader.ReadInt()
        };
    }

    public static void WriteCurioComponentDescriptor(this NetworkWriter writer, CurioComponentDescriptor target)
    {
        writer.WriteInt(target.UsesLeft);
    }
}