using System.IO;

namespace Frontend.Script;

public class ProgramInitializer
{
    public static void DirectoryCreator()
    {
        if (!Directory.Exists("Image"))
            Directory.CreateDirectory("Image");
        
        if (!Directory.Exists("Image/Temporary"))
            Directory.CreateDirectory("Image/Temporary");
        
        if (!Directory.Exists("Image/Static"))
            Directory.CreateDirectory("Image/Static");
    }
}