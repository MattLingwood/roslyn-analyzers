﻿using System;

namespace TestData.Enums.DefaultLabelShouldBeLastInSwitchStatement.DiagnosticAnalyzer
{
    public class EnumerationWithDefaultSwitchNotLast
    {
        public void EnumerationMethod(CarModels carModel)
        {
            switch (carModel)
            {
                default:
                    throw new ArgumentOutOfRangeException(nameof(carModel), carModel, null);
                case CarModels.Ferrari:
                    break;
                case CarModels.Lamborghini:
                    break;
                case CarModels.Mercedes:
                    break;
            }
        }

        public enum CarModels
        {
            Ferrari,
            Lamborghini,
            Mercedes
        }
    }
}
