# Mobile-App-NFC
**Current Status**
Android: Mostly working. Currently only prints out the node configuration into the console and does not display it (should be fairly simple to implement). The app also does not check if you are on the read/write page and currently you need to change a bool depending on what functionality you want.

iOS: All of the logic is implemented however there is currently an error that occurs when trying to parse the second NdefMessage through the protocol buffer. Will have this fixed ASAP. The node config is also currently displayed in the console instead of on the read page.
