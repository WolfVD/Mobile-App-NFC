using System.Threading.Tasks;

namespace NFCProject.Services
{
    public interface IWriteScan
    {
        void StartWriteScan(string NetID, string NetChan, string NodeConfig, string OperMode, string EncKey, string AuthKey, string UpdateRate, bool NetIDOn, bool NetChanOn, bool NodeConfigOn, bool OperModeOn, bool EncKeyOn, bool AuthKeyOn, bool UpdateRateOn);
    }
}
