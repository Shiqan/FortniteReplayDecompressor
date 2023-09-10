using Unreal.Core;

namespace FortniteReplayReader.Models.NetFieldExports;

public class PlayerNameData
{
    public byte Handle { get; private set; }
    public byte Unknown1 { get; private set; }
    public bool IsPlayer { get; private set; }
    public string EncodedName { get; private set; }
    public string DecodedName { get; private set; }

    public PlayerNameData(FArchive archive)
    {
        Handle = archive.ReadByte();
        Unknown1 = archive.ReadByte();
        IsPlayer = archive.ReadBoolean();
        EncodedName = archive.ReadFString();

        var decodedName = string.Empty;

        if (IsPlayer)
        {
            for (var i = 0; i < EncodedName.Length; i++)
            {
                var shift = (EncodedName.Length % 4 * 3 % 8 + 1 + i) * 3 % 8;
                decodedName += (char) (EncodedName[i] + shift);
            }

            DecodedName = decodedName;
        }
        else
        {
            DecodedName = EncodedName;
        }
    }
}
