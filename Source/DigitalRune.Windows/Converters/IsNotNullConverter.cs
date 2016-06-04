﻿// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.Globalization;
using System.Windows.Data;


namespace DigitalRune.Windows
{
    /// <summary>
    /// Converts an <see cref="object"/> to a <see cref="bool"/> by checking whether the reference
    /// is not <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// The converter returns <see langword="false"/> if the value is <see langword="null"/>;
    /// otherwise <see langword="true"/>.
    /// </remarks>
#if !SILVERLIGHT && !WINDOWS_PHONE
    [ValueConversion(typeof(object), typeof(bool))]
#endif
    public class IsNotNullConverter : IValueConverter
    {
        /// <summary>
        /// Gets an instance of the <see cref="IsNotNullConverter"/>.
        /// </summary>
        /// <value>An instance of the <see cref="IsNotNullConverter"/>.</value>
        public static IsNotNullConverter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new IsNotNullConverter();

                return _instance;
            }
        }
        private static IsNotNullConverter _instance;


        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns <see langword="null"/>, the valid null value is
        /// used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }


        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns <see langword="null"/>, the valid null value is
        /// used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
