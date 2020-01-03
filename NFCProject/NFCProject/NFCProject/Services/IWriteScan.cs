using System.Threading.Tasks;

namespace NFCProject.Services
{
    public interface IWriteScan
    {
        Task StartWriteScan(string netID, string netChan, string configID, string encKeyCom, string authKeyCom, string encKeyOTAP, string authKeyOTAP, string operMode);
    }
}
