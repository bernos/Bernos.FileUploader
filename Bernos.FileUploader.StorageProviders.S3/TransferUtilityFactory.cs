using Amazon.S3;
using Amazon.S3.Transfer;

namespace Bernos.FileUploader.StorageProviders.S3
{
    public class TransferUtilityFactory
    {
        public TransferUtility CreateTransferUtility(AmazonS3Client client)
        {
            return new TransferUtility(client);
        }
    }
}