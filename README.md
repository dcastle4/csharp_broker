# Broker for CpE4020 Mini-Project

This C#/.NET based program implements a broker for our simple pub/sub architecture created for our mini-project in CpE4020:Device Networks. 

It primarily runs two things: a TCP listener/server, and an HTTP server. The TCP server connects directly to the publisher (hosted on a BeagleBone Black over USB ethernet) and retrieves the sensor data as a stringified JSON. The HTTP server then displays this data, allowing for anyone with access to our public IP address and port to retrieve our sensor data over the internet.
 
Here is an example screenshot of our Broker running in a terminal using MonoDevelop:

![broker](doc/broker.png?raw=true)
