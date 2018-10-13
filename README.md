# TopShelpServiceWithCancellation
A POC of a .net core console app, hosted as a Windows service with TopShelf.  Cancellation allows a graceful shutdown.  TopShelf is an open-source project by Chris Patterson, Travis Smith, and Dru Sellers.  It allows you to host simple .net console apps as powerful Windows services.  Most of the code in Program.cs was taken from the documentation at the TopShelf project page.  Check it out here:  http://docs.topshelf-project.com.

In this program, TopShelf is referenced as a nuget package.  I am using TopShelf to host a service named TopShelfService which runs a simple task named TaskWithCancellation every 17 seconds.  When TaskWithCancellation runs, a cancellation token is passed into it.  When the TopShelf service is stopped, a request is made to cancel the token.  The TaskWithCancellation checks periodically if there was a request to cancel the token.  If it finds that cancellation was requested it begins a cleanup routine.  The program waits for 5 seconds for the task to clean up and then the service stops.  You can verify that it was able to complete the cleanup by checking an output file named fileTemp.txt in the executable directory.  The file contains a play-by-play of what happened when the program was running and if the service was stopped in the middle of doing its work it lists what it did during cleanup.

Output when the service is stopped while it is in the middle of working:

    Begin
    Friday, October 12, 2018 1:02:39 PM
    10/12/2018 1:02:39 PM processing 0 of 5
    10/12/2018 1:02:39 PM processing 1 of 5
    10/12/2018 1:02:40 PM processing 2 of 5
    10/12/2018 1:02:40 PM processing 3 of 5
    10/12/2018 1:02:41 PM Cancellation is requested...
    10/12/2018 1:02:41 PM cleaning up
    10/12/2018 1:02:43 PM done cleaning up

Output when the service is stopped when it is not currently working:

    Begin
    Friday, October 12, 2018 1:04:43 PM
    10/12/2018 1:04:43 PM processing 0 of 5
    10/12/2018 1:04:43 PM processing 1 of 5
    10/12/2018 1:04:44 PM processing 2 of 5
    10/12/2018 1:04:44 PM processing 3 of 5
    10/12/2018 1:04:45 PM processing 4 of 5
    10/12/2018 1:04:45 PM End

To compile as an EXE:

    dotnet publish --self-contained --runtime win81-x64
Install:

    TopShelfServiceWithCancellation.exe install -servicename:TopShelfService
    
Start:

    sc start TopShelfService 
Stop:

    sc stop TopShelfService

