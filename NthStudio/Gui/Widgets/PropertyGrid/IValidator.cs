using System;

namespace NthStudio.Gui.Widgets.PropertyGrid
{
    public interface IValidator
    {
        /// <summary>
        /// Returns the validated value or null if the value was not valid.
        /// </summary>
        /// <param name="o">The Object to validate</param>
        /// <returns>The type of the returned object must be the same as ValidatedType</returns>
        Object ValidateValue(Object o);

        /// <summary>
        /// Tries to convert an object of the validated type to the given type T.
        /// </summary>
        /// <typeparam name="T">The target type of the conversion.</typeparam>
        /// <param name="o">The object of the validated type that should be converted.</param>
        /// <returns>The converted object or null if the conversion failed.</returns>
        T ConvertTo<T>(Object o) where T : class;

        /// <summary>
        /// The type validated by this validator.
        /// </summary>
        Type ValidatedType { get; }

        /// <summary>
        /// An error message specifying the last error. Empty if the last validation
        /// was successful.
        /// </summary>
        String Message { get; }

        /// <summary>
        /// A default value of the validated type. This default value must always
        /// pass validation.
        /// </summary>
        Object DefaultValue { get; }
    }
}
