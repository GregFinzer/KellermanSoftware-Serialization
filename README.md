[<img src="https://github.com/GregFinzer/comparenetobjects/blob/master/logo.png">](http://www.kellermansoftware.com)

# Project Description
Binary Serialization, Compression, and Encryption Library for .NET.  Chief benefit is that you do not need to decorate your classes with any [Serializable] attribute.  

# Compatibility
Compatible with .NET Framework 4.0 and higher with builds for 4.5, 4.5.1, 4.5.2, 4.6.  .NET Standard 2.0 which means it will work for Mono 5.4, Xamarin iOS 10.14, Xamarin.Mac 3.8, Xamarin.Android 8.0, UWP 10.0.16299

# NuGet Package
<a href="https://www.nuget.org/packages/KellermanSoftware.Serialization">
  <img src="http://img.shields.io/nuget/v/KellermanSoftware.Serialization.svg" alt="NuGet Version" height="50">
</a>

<a href="https://www.nuget.org/packages/KellermanSoftware.Serialization">
  <img src="https://img.shields.io/nuget/dt/KellermanSoftware.Serialization.svg" alt="NuGet Downloads" height="50">
</a>

https://www.nuget.org/packages/KellermanSoftware.Serialization

## Installation

Install with NuGet Package Manager Console
```
Install-Package KellermanSoftware.Serialization
```

Install with .NET CLI
```
dotnet add package KellermanSoftware.Serialization
```
# Features
* Serialize and deserialize objects without the BinaryFormatter or JSON.NET.  Classes do not need to be marked as serializable
* Compress bytes or files using MiniLZO compression
* Encrypt strings, files, or bytes using AES Encryption



# Licensing
https://github.com/GregFinzer/KellermanSoftware-Serialization/blob/master/LICENSE
