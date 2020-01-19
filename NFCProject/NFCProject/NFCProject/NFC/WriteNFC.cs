using Google.Protobuf;
using NFCProject.Pages;
using NFCProject.Services;
using System;

namespace NFCProject
{
    public class WriteNFC
    {
        public static byte[] EncryptNonceWrite(byte[] input) // Encrypt the input (nonce) and create the write payload
        {

            RX1_NFC_Reply nfcReply = RX1_NFC_Reply.Parser.ParseFrom(input);
            byte[] nonce = nfcReply.Nonce.ToByteArray();

            byte[] Key = hexToByte("2b7e151628aed2a6abf7158809cf4f3c"); //Convert the hex string to a byte array
            byte[] IV = hexToByte("000102030405060708090a0b0c0d0e0f"); //Convert the hex string to a byte array

            CryptoHandler cryptoHandler = new CryptoHandler();

            byte[] encryptedNonce = cryptoHandler.Encrypt(nonce, Key, IV); //Encrypt the nonce using AES128 CBC encryption (with PKCS7Padding)

            byte[] trimmedResult = new byte[16];

            for (int i = 0; i < 16; i++)
            {
                trimmedResult[i] = encryptedNonce[i];
            }

            // Get values from entries on write page
            WriteToNode writeNodePage = new WriteToNode();
            bool[] checkedList = writeNodePage.ReturnChecked();
            string[] valueList = writeNodePage.ReturnValues();

            uint NetID = Convert.ToUInt32(valueList[0]);
            uint NetChan = Convert.ToUInt32(valueList[1]);
            string NodeConfig = valueList[2];
            string OperMode = valueList[3];
            ByteString EncKey = ByteString.CopyFrom(hexToByte(valueList[4]));
            ByteString AuthKey = ByteString.CopyFrom(hexToByte(valueList[5]));

            bool NetIDBool = checkedList[0];
            bool NetChanBool = checkedList[1];
            bool NodeConfigBool = checkedList[2];
            bool OperModeBool = checkedList[3];
            bool EncKeyBool = checkedList[4];
            bool AuthKeyBool = checkedList[5];

            NodeConfiguration nodeConfiguration;
            NodeOperatingMode operatingMode;

            #region optimize this (if possible)


            if (NodeConfig == "0")
            {
                nodeConfiguration = NodeConfiguration.Desk1M;
            }
            else if (NodeConfig == "1")
            {
                nodeConfiguration = NodeConfiguration.Desk2M;
            }
            else if (NodeConfig == "2")
            {
                nodeConfiguration = NodeConfiguration.Ceiling1M;
            }
            else
            {
                nodeConfiguration = NodeConfiguration.Ceiling2M;
            }

            if (OperMode == "0")
            {
                operatingMode = NodeOperatingMode.Run;
            }
            else
            {
                operatingMode = NodeOperatingMode.Inventory;
            }
            #endregion fix garbage later

            //Create a request with the encrypted nonce and values from write page
            RX1_NFC_Request nfcRequest = new RX1_NFC_Request
            {
                RequestType = RX1_NFC_Request.Types.NFCRequestType.SetNodeConfig,
                EncryptedNonce = ByteString.CopyFrom(trimmedResult),
                NodeConfig = new RX1_NFC_Config
                {
                    NetworkID = NetID,
                    HasNetworkID = NetIDBool,
                    NetworkChannel = NetChan,
                    HasNetworkChannel = NetChanBool,
                    NodeConfiguration = nodeConfiguration,
                    HasNodeConfiguration = NodeConfigBool,
                    OperatingMode = operatingMode,
                    HasOperatingMode = OperModeBool,
                    EncryptionKey = EncKey,
                    HasEncryptionKey = EncKeyBool,
                    AuthenticationKey = AuthKey,
                    HasAuthenticationKey = AuthKeyBool

                },
                Chksum = calculateChksum(valueList[0], valueList[1], valueList[2], valueList[3], valueList[4], valueList[5], checkedList[0], checkedList[1], checkedList[2], checkedList[3], checkedList[4], checkedList[5])

            };

            byte[] bytes = nfcRequest.ToByteArray();

            return bytes;

        }

        //Calculate the checksum to ensure that the message was written succesfuly
        static uint calculateChksum(string NetID, string NetChan, string NodeConfig, string OperMode, string EncKey, string AuthKey, bool NetIDBool, bool NetChanBool, bool NodeConfigBool, bool OperModeBool, bool EncKeyBool, bool AuthKeyBool)
        {
            int chksum = 0;

            chksum += Convert.ToInt32(NetID);
            chksum += Convert.ToInt32(NetChan);
            chksum += Convert.ToInt32(NodeConfig);
            chksum += Convert.ToInt32(OperMode);
            for (int i = 0; i < 16; i++)
            {
                chksum += hexToByte(EncKey)[i] & 0xff;
            }
            for (int i = 0; i < 16; i++)
            {
                chksum += hexToByte(AuthKey)[i] & 0xff;
            }

            chksum += NetIDBool ? 1 : 0;
            chksum += NetChanBool ? 1 : 0;
            chksum += NodeConfigBool ? 1 : 0;
            chksum += OperModeBool ? 1 : 0;
            chksum += EncKeyBool ? 1 : 0;
            chksum += AuthKeyBool ? 1 : 0;

            return Convert.ToUInt32(chksum);
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
