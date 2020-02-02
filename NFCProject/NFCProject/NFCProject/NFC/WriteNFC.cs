using Google.Protobuf;
using NFCProject.Pages;
using NFCProject.Services;
using System;

namespace NFCProject
{
    public class WriteNFC
    {

        public static byte[] CreateRequest()
        {
            WriteToNode writeToNode = new WriteToNode();
            string[] valueList = writeToNode.ReturnValues();
            bool[] checkedList = writeToNode.ReturnChecked();

            NodeConfigRequest request = new NodeConfigRequest
            {
                NetworkID = Convert.ToInt32(valueList[0]),
                HasNetworkID = checkedList[0],
                NetworkChannel = Convert.ToInt32(valueList[1]),
                HasNetworkChannel = checkedList[1],
                NodeConfiguration = SensorView._config,
                HasNodeConfiguration = checkedList[2],
                OperatingMode = GetOperatingMode(Convert.ToInt32(valueList[3])),
                HasOperatingMode = checkedList[3],
                EncryptionKey = ByteString.CopyFrom(hexToByte(valueList[4])),
                HasEncryptionKey = checkedList[4],
                AuthenticationKey = ByteString.CopyFrom(hexToByte(valueList[5])),
                HasAuthenticationKey = checkedList[5],
                UplinkRate = Convert.ToInt32(valueList[6]),
                HasUplinkRate = checkedList[6],
                NodeRole = GetNodeRole(Convert.ToInt32(valueList[7])),
                HasNodeRole = checkedList[7],
                AssetTrackingEnabled = Convert.ToInt32(valueList[8])==0,
                HasAssetTrackingEnabled = checkedList[8],
                FeatureLock = Convert.ToInt32(valueList[9]) == 0,
                HasFeatureLock = checkedList[9],
                Delay = 1
            };

            byte[] bytes = request.ToByteArray();
            byte[] paddedBytes;

            // Pad if the result is not % 16
            if ((bytes.Length % 16) != 0)
            {
                paddedBytes = new byte[bytes.Length + (16 - (bytes.Length % 16))];
                Array.Copy(bytes, 0, paddedBytes, 0, bytes.Length);
                for (int i = bytes.Length; i < paddedBytes.Length; i++)
                {
                    paddedBytes[i] = 0;
                }
            }
            else
            {
                paddedBytes = new byte[bytes.Length];
                Array.Copy(bytes, 0, paddedBytes, 0, bytes.Length);
            }

            CryptoHandler cryptoHandler = new CryptoHandler();

            byte[] Key = hexToByte("2b7e151628aed2a6abf7158809cf4f3c"); //Convert the hex string to a byte array
            byte[] IV = hexToByte("000102030405060708090a0b0c0d0e0f"); //Convert the hex string to a byte array
            byte[] encryptedBytes = cryptoHandler.Encrypt(paddedBytes, Key, IV);

            return encryptedBytes;
        }

        private static NodeOperatingMode GetOperatingMode(int selection)
        {
            switch (selection)
            {
                case 0:
                    return NodeOperatingMode.Run;
                case 1:
                    return NodeOperatingMode.Inventory;
            }
            return NodeOperatingMode.Run;
        }

        private static NodeRole GetNodeRole(int selection)
        {
            switch (selection)
            {
                case 0:
                    return NodeRole.HeadnodeAnchor;
                case 1:
                    return NodeRole.SubnodeTracked;
            }
            return NodeRole.HeadnodeAnchor;
        }

        public static byte[] hexToByte(String s)
        {
            int length = s.Length;
            byte[] data = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                data[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
            }

            return data;
        }

    }
}
