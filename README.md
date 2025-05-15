# TestBench Target UWP

## Purpose
A sample application designed to serve as a testing subject for developers creating monitoring, accessibility, or UI automation tools. This application provides predictable user interface elements and behaviors that developers can use to test their monitoring solutions.
A Universal Windows Platform (UWP) version of the TestBench Target application designed for the Microsoft Store.

## Main Features
- Small and fast application with consistent behavior
- Tests opening a Windows directory in the Documents folder
- Provides a target app for trying out monitoring and testing tools
- Simulates adding defined items to a table
- Simple chronological display of data in a table format
- Data persistence via JSON files
- Customizable table values with validation

## Technical Details
- Designed with accessibility and testing in mind
- UWP - Universal Windows Platform framework
- Uses Newtonsoft.Json for data serialization
- Full compatibility with Windows 10 and 11
- Developed for .NET 8

## Features
- Named UI elements for easy recognition in tested third-party applications
- Display and manage chronological data in a table format
- Add, delete, and save data entries
- Modern UI with support for high DPI displays

## Usage
1. Start the application
2. Click on "Open Application" on the main screen
3. In the second window, you can:
   - Select dates from the dropdown - use the arrow keys and also the mouse wheel
   - Add entries with procedure names, points, and delegate information
   - Save and load data to/from JSON files
   - Delete selected entries
   - Open the Documents folder to access saved data

## Requirements
- Windows 10 (version 1803 or higher)
- Visual Studio 2022 with UWP development workload

## For Developers
This application is ideal for testing:
- UI automation tools
- Accessibility solutions
- Application monitoring systems
- UI interaction recording tools
- Form control validation

## Development
This project uses the MVVM pattern and is designed for submission to the Microsoft Store.

## License
This project is licensed under the Apache License 2.0 - see the LICENSE file for details.

## Author
© 2025 Rudolf Mendzezof
