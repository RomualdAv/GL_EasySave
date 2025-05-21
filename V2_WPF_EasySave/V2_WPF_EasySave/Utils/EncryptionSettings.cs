using System.Collections.Generic;

namespace V2_WPF_EasySave.Utils
{
    public static class EncryptionSettings
    {
        public static List<string> ExtensionsToEncrypt { get; set; } = new() { ".txt", ".pdf" };
        public static string Key { get; set; } = "1234567890123456"; // 16 caractères = 128 bits
    }
}
