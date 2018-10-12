sc stop TopShelfService

echo on

dotnet publish .\src\TopShelfServiceWithCancellation\TopShelfServiceWithCancellation.csproj --self-contained --runtime win81-x64

