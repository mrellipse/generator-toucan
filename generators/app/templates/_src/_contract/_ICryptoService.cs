using System;

namespace <%=assemblyName%>.Contract
{
    public interface ICryptoService
    {
        bool CheckKey(string hash, string salt, string data);
        string CreateKey(string salt, string data);
        string CreateKey(string salt, string data, int keySizeInKb);
        string CreateFingerprint(string data);
        string CreateSalt();
        string CreateSalt(int sizeInKb);
    }
}