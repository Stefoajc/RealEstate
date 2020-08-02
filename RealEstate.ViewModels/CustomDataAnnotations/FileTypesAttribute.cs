using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RealEstate.WebAppMVC.Helpers.DataAnnotations
{
    //public class FileTypesAttribute : ValidationAttribute
    //{
    //    private readonly List<string> _types;

    //    public FileTypesAttribute(string types)
    //    {
    //        _types = types.Split(',').ToList();
    //    }

    //    public override bool IsValid(object value)
    //    {
    //        if (value == null) return true;

    //        var fileExt = System.IO
    //            .Path
    //            .GetExtension((value as
    //                HttpPostedFileBase).FileName).Substring(1);
    //        return _types.Contains(fileExt, StringComparer.OrdinalIgnoreCase);
    //    }

    //    public override string FormatErrorMessage(string name)
    //    {
    //        return $"Invalid file type. Only the following types {String.Join(", ", _types)} are supported.";
    //    }
    //}

    //public class FileListTypesAttribute : ValidationAttribute
    //{
    //    private readonly List<string> _types;

    //    public FileListTypesAttribute(string types)
    //    {
    //        _types = types.Split(',').ToList();
    //    }

    //    public override bool IsValid(object value)
    //    {
    //        if (value == null) return true;

    //        if (!(value is HttpPostedFileBase[] fileList)) return false;

    //        foreach (var file in fileList)
    //        {
    //            var fileExtWithDot = System.IO.Path.GetExtension(file.FileName) ?? "";
    //            var fileExtWithoutDot = fileExtWithDot.Substring(1);

    //            if (!(_types.Contains(fileExtWithDot, StringComparer.OrdinalIgnoreCase)
    //                || _types.Contains(fileExtWithoutDot, StringComparer.OrdinalIgnoreCase)))
    //            {
    //                return false;
    //            }
    //        }

 
    //        return true;
    //    }

    //    public override string FormatErrorMessage(string name)
    //    {
    //        return $"Invalid file type. Only the following types {String.Join(", ", _types)} are supported.";
    //    }
    //}
}