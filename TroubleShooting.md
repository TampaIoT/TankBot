# Trouble Shooting

You will know that the Firmware is operaing properly on the TankBot if the LEDs on the sensor board are flashing rapidly, between 5 and 10 times a second for a very samll fraction of a second.

If the LEDs flash 5 times then turn off for about 2 seconds your TankBot is not connected to the internet.  Make sure you can connect to your Windows 10 IoT Core devices [Device Portal](https://developer.microsoft.com/en-us/windows/iot/docs/deviceportal). 

If the LEDs flash twice then turn off for about 2 seconds your TankBot is not communicating witht he underlying robot platform, check the serial lines between the Windows 10 IoT Core board to the robot platform. [Connecting Windows 10 IoT Core to Robot Platform](SerialConnections.md)

If you install the [Custom Firmware](https://github.com/TampaIoT/TankBot/tree/master/src/mBotFirmware)  on your mBot ([with instuctions found here](https://github.com/TampaIoT/TankBot/blob/master/GettingStarted.md)).  Your mBot will use it's onboard RGB LEDs to show the status.
- No LEDs or Dim Red: mBot is alive but not connected to Windows 10 IoT Core Firmware
- Bright Yellow: Windows 10 IoT Core Connected to mBot, waiting for client connection
- Bright Green: Device Application Connected