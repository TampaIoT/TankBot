# User Interface Requirements
The intent of this document is define the User Inteface requirements for the applications that make up the Tampa IoT Tank Bot System.

## Desktop/Tablet/Hololens Controller App
The controller App will be used to Connect to and Control a single Tank Bot. It will be used to move, configure and monitor state of the bot.  

#### Connection Process
1) User Presses Connected
2) Finds all "Registered Bots, either by BlueTooth, WiFi or Cloud Connection
3) Finds the bot they want to connect to.
4) Presses Connect
5) Prompted for PIN for the Bot
6) If valid sets up connection.  Will remember PIN for future connections.
7) Sets up a list of "Favorite" bots for easy connection

#### Navigation/Control Movement
1) Will use XBox Controller to Manipulate Bot
2) Will use Arrow Keys to Manipulate Bot
3) Will use Buttons on UI to Manipulate Bot
4) Can "plan route" as a set of vectors.

#### Status Reporting
1) Will display a list of sensors that are reporting, this will be dynamic
2) Will display the status of all Proximity sensors if available
3) Will display heading
4) Will display current known location on "grid" (if available)
5) Will display turret heading

## Raspberry Pi (Usually via Remote Desktop or Small LCD Display)
In some cases the Rasbperry Pi may have a small LCD screen attached, you can also "remote" in to the Raspberry Pi to display the user inteface that would be shown if the device had a "head".

The firmware application will have basic buttons to control navigation and report sensors and be similar to the Primary Controller App

## Swarm Controller App