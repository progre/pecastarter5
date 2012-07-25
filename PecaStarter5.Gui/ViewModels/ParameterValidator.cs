using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Progressive.Peercast4Net.Utils;
using Progressive.Peercast4Net.Commons.Utils;

namespace Progressive.PecaStarter5.ViewModels
{
    public static class ParameterValidator
    {
        private const int MaxParameterLength = 246;

        public static ValidationResult ValidateParameter(string value)
        {
            return PeercastUtils.PercentEncode(value).Length <= MaxParameterLength ? ValidationResult.Success : new ValidationResult("文字数の制限を超えています");
        }

        public static ValidationResult ValidateChannelName(string value)
        {
            return HttpUtils.ToRfc3986(value, Encoding.UTF8).Length <= MaxParameterLength ? ValidationResult.Success : new ValidationResult("文字数の制限を超えています");
        }
    }
}
