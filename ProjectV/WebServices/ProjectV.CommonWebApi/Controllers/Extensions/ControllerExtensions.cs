using System;
using Acolyte.Assertions;
using Microsoft.AspNetCore.Mvc;

namespace ProjectV.CommonWebApi.Controllers.Extensions
{
    public static class ControllerExtensions
    {
        public static string ControllerSuffix { get; } = nameof(Controller);

        public static StringComparison DefaultComparison { get; } = StringComparison.Ordinal;


        public static string GetControllerNameFromType<TController>()
            where TController : Controller
        {
            return GetControllerNameFromType(typeof(TController));
        }

        public static string GetControllerNameFromType(this Type controllerType)
        {
            controllerType.ThrowIfNull(nameof(controllerType));

            return GetControllerNameFromTypeName(controllerType.Name);
        }

        public static string GetControllerNameFromTypeName(this string controllerTypeName)
        {
            controllerTypeName.ThrowIfNullOrWhiteSpace(nameof(controllerTypeName));

            // Remove Controller part from the end.
            if (controllerTypeName.EndsWith(ControllerSuffix, DefaultComparison))
            {
                int startIndex = controllerTypeName.Length - ControllerSuffix.Length;
                return controllerTypeName.Remove(startIndex);
            }

            return controllerTypeName;
        }
    }
}
