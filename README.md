# wii-capture
A WPF application to capture WII Balance Board Data to File including COP Calculations.

Supports up to 2 balance boards.

Stores Raw & Calibrated Force Data to File.

Sensor calibration files can be provided in the bin/Debug/calibration sub folder. Ensure that the name of the file matches the device id. e.g. [HIDSerial].xml, it will then be automatically loaded by the software. Some sample calibration files have been provided.

# WiiMoteLib
The source contains a modified version of https://github.com/BrianPeek/WiimoteLib. The modification exposes the HIDSerial property. This is unique to the WII Balance Board and used to identify the board in the software when using multiple devices.

# Output Files
Output files contain the following columns

* Absolute	- Capture time of frame
* Relative	- Time since start of capture
* BL	- Raw force in Newtons - Bottom Left Sensor
* TL	- Raw force in Newtons - Top Left Sensor
* BR	- Raw force in Newtons - Bottom Right Sensor
* TR	- Raw force in Newtons - Top Right Sensor
* BLC	- Calibrated force in Newtons - Bottom Left Sensor
* TLC	- Calibrated force in Newtons - Top Left Sensor
* BRC	- Calibrated force in Newtons - Bottom Right Sensor
* TRC	- Calibrated force in Newtons - Top Right Sensor
* COPX	- Calibrated Center of Pressure - X coordinate
* COPY	- Calibrated Center of Pressure - Y coordinate
* MX	- Moment Force X
* MY	- Moment Force Y
* MZ	- Unavailable from WII balance board
* Weight - Weight in KG
