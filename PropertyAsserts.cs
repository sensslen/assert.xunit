#pragma warning disable CA1052 // Static holder types should be static
#pragma warning disable CA1720 // Identifier contains type name
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task

#if XUNIT_NULLABLE
#nullable enable
#else
// In case this is source-imported with global nullable enabled but no XUNIT_NULLABLE
#pragma warning disable CS8622
#endif

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace Xunit
{
#if XUNIT_VISIBILITY_INTERNAL
	internal
#else
	public
#endif
	partial class Assert
	{
		/// <summary>
		/// Verifies that the provided object raised <see cref="INotifyPropertyChanged.PropertyChanged"/>
		/// as a result of executing the given test code.
		/// </summary>
		/// <param name="object">The object which should raise the notification</param>
		/// <param name="propertyName">The property name for which the notification should be raised</param>
		/// <param name="testCode">The test code which should cause the notification to be raised</param>
		/// <exception cref="PropertyChangedException">Thrown when the notification is not raised</exception>
		public static void PropertyChanged(
			INotifyPropertyChanged @object,
			string propertyName,
			Action testCode)
		{
			GuardArgumentNotNull(nameof(@object), @object);
			GuardArgumentNotNull(nameof(propertyName), propertyName);
			GuardArgumentNotNull(nameof(testCode), testCode);

			var propertyChangeHappened = false;

#if XUNIT_NULLABLE
			void handler(object? sender, PropertyChangedEventArgs args) =>
#else
			void handler(object sender, PropertyChangedEventArgs args) =>
#endif
				propertyChangeHappened = propertyChangeHappened || string.IsNullOrEmpty(args.PropertyName) || propertyName.Equals(args.PropertyName, StringComparison.OrdinalIgnoreCase);

			@object.PropertyChanged += handler;

			try
			{
				testCode();
				if (!propertyChangeHappened)
					throw PropertyChangedException.ForUnsetProperty(propertyName);
			}
			finally
			{
				@object.PropertyChanged -= handler;
			}
		}

		/// <summary/>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("You must call Assert.PropertyChangedAsync (and await the result) when testing async code.", true)]
		public static void PropertyChanged(
			INotifyPropertyChanged @object,
			string propertyName,
			Func<Task> testCode)
		{
			throw new NotImplementedException("You must call Assert.PropertyChangedAsync (and await the result) when testing async code.");
		}

		/// <summary>
		/// Verifies that the provided object raised <see cref="INotifyPropertyChanged.PropertyChanged"/>
		/// as a result of executing the given test code.
		/// </summary>
		/// <param name="object">The object which should raise the notification</param>
		/// <param name="propertyName">The property name for which the notification should be raised</param>
		/// <param name="testCode">The test code which should cause the notification to be raised</param>
		/// <exception cref="PropertyChangedException">Thrown when the notification is not raised</exception>
		public static async Task PropertyChangedAsync(
			INotifyPropertyChanged @object,
			string propertyName,
			Func<Task> testCode)
		{
			GuardArgumentNotNull(nameof(@object), @object);
			GuardArgumentNotNull(nameof(propertyName), propertyName);
			GuardArgumentNotNull(nameof(testCode), testCode);

			var propertyChangeHappened = false;

#if XUNIT_NULLABLE
			void handler(object? sender, PropertyChangedEventArgs args) =>
#else
			void handler(object sender, PropertyChangedEventArgs args) =>
#endif
				propertyChangeHappened = propertyChangeHappened || string.IsNullOrEmpty(args.PropertyName) || propertyName.Equals(args.PropertyName, StringComparison.OrdinalIgnoreCase);

			@object.PropertyChanged += handler;

			try
			{
				await testCode();
				if (!propertyChangeHappened)
					throw PropertyChangedException.ForUnsetProperty(propertyName);
			}
			finally
			{
				@object.PropertyChanged -= handler;
			}
		}
	}
}
