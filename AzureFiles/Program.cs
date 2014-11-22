using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            var credentials = new StorageCredentials("sqlsat355parma", "lgFYPZgzyqjZun5+1h7JxU6MKrXz+j9/eL4ZPj8+Gg2Xe79EYoFUT3b7gql9chrudlntXTwU8VAR8IFJ77ZqCw==");
            var account = new CloudStorageAccount(credentials, true);
            var file = account.CreateCloudFileClient();
            var share = file.GetShareReference("files");
            share.CreateIfNotExists();
        }
    }
}
