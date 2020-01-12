# Current Status
Android: Fully functional.

iOS: All of the logic is implemented however there is currently an error that occurs when trying to parse the second NFDEFMessage Payload through the protocol buffer. For some reason, the length of the payload is much longer than it should be (143 bytes instead of the intended 33).
