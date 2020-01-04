using System.Threading.Tasks;

namespace NFCProject.Services
{
    public interface IWriteScan
    {
        Task StartWriteScan(string netIDFinal, string netChanFinal, string NodeConfigFinal, string OperModeFinal, string EncKeyFinal, string UpdateRateFinal);
    }
}
