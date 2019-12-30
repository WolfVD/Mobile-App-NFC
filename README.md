# Mobile-App-NFC
**Currently does not include protocoll buffer/writing**
Due to the simulated and physical NFC working differently for IOS, a scan button had to be used to initiate the scan. (The physical NFC previously couldn't be tested because a developer license was required)
Due to this differing from the way it is done in Android, the protocol buffer, which was initially in the shared code, now has to be done seperately for each platform. The protocol buffers haven't been implemented for each platform, but will be ASAP and once this has been done writing functionality will be added again (The writing functionality is still present but isn't called)
