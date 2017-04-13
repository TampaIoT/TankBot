# Projects that Make up the Laser Tank Bot Platform

### TampaIoT.TankBot.Core
Provides core data strucutres, models and interfaces that a TankBot implementation must implement.

### TampaIoT.TankBot.Cloud
Provides a mechanism to interface with Cloud Services, Specifically Azure IoT Hub.

### TampaIoT.TankBot.UWP.Core
Provides shared functionalty used by the Desktop/Tablet/Phone app and Windows 10 IoT Core Rapberry Pi Firmware

### TampaIoT.TankBot.mBot
Provide an implementation for the TankBot Core for the MakeBlocks mBot Robotics Platform for the Basic, Ranger and Rover Models

### TampaIoT.TankBot.Firmware
Provides the functionality that runs on the Windows 10 IoT Core device.  This firmware interfaces with the specific Robot drive platform in which the mBot platform is currently supported.

### TampaIoT.TankBot.App
Consists of a UWP app that will run on a Desktop, Tablet, Phone or even a dedicated Win 10 IoT Core device with a Display to manage and control our Robots.
