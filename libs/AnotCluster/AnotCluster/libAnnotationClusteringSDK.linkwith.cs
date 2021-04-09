using System;
using ObjCRuntime;

[assembly: LinkWith ("libAnnotationClusteringSDK.a", LinkTarget.Simulator | LinkTarget.ArmV7, SmartLink = true, ForceLoad = true)]
