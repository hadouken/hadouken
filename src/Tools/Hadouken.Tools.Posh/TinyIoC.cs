//===============================================================================
// TinyIoC
//
// An easy to use, hassle free, Inversion of Control Container for small projects
// and beginners alike.
//
// https://github.com/grumpydev/TinyIoC
//===============================================================================
// Copyright © Steven Robbins.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

#region Preprocessor Directives

// Uncomment this line if you want the container to automatically
// register the TinyMessenger messenger/event aggregator
//#define TINYMESSENGER

// Preprocessor directives for enabling/disabling functionality
// depending on platform features. If the platform has an appropriate
// #DEFINE then these should be set automatically below.
#define EXPRESSIONS
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

// Platform supports System.Linq.Expressions
#define COMPILED_EXPRESSIONS // Platform supports compiling expressions
#define APPDOMAIN_GETASSEMBLIES // Platform supports getting all assemblies from the AppDomain object
#define UNBOUND_GENERICS_GETCONSTRUCTORS // Platform supports GetConstructors on unbound generic types
#define GETPARAMETERS_OPEN_GENERICS // Platform supports GetParameters on open generics
#define RESOLVE_OPEN_GENERICS // Platform supports resolving open generics
#define READER_WRITER_LOCK_SLIM // Platform supports ReaderWriterLockSlim

//// NETFX_CORE
//#if NETFX_CORE
//#endif

// CompactFramework / Windows Phone 7
// By default does not support System.Linq.Expressions.
// AppDomain object does not support enumerating all assemblies in the app domain.
#if PocketPC || WINDOWS_PHONE
#undef EXPRESSIONS
#undef COMPILED_EXPRESSIONS
#undef APPDOMAIN_GETASSEMBLIES
#undef UNBOUND_GENERICS_GETCONSTRUCTORS
#endif

// PocketPC has a bizarre limitation on enumerating parameters on unbound generic methods.
// We need to use a slower workaround in that case.
#if PocketPC
#undef GETPARAMETERS_OPEN_GENERICS
#undef RESOLVE_OPEN_GENERICS
#undef READER_WRITER_LOCK_SLIM
#endif

#if SILVERLIGHT
#undef APPDOMAIN_GETASSEMBLIES
#endif

#if NETFX_CORE
#undef APPDOMAIN_GETASSEMBLIES
#undef RESOLVE_OPEN_GENERICS
#endif

#if COMPILED_EXPRESSIONS
#define USE_OBJECT_CONSTRUCTOR
#endif

#endregion

namespace Hadouken.Tools.Posh {
    #if EXPRESSIONS
    using System.Linq.Expressions;
    using System.Threading;

#endif

#if NETFX_CORE
	using System.Threading.Tasks;
	using Windows.Storage.Search;
    using Windows.Storage;
	using Windows.UI.Xaml.Shapes;
#endif

    #region SafeDictionary

#if READER_WRITER_LOCK_SLIM
#if TINYIOC_INTERNAL
    internal
#else
    public
#endif
        class SafeDictionary<TKey, TValue> : IDisposable {
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();
        private readonly ReaderWriterLockSlim _padlock = new ReaderWriterLockSlim();

        public TValue this[TKey key] {
            set {
                this._padlock.EnterWriteLock();

                try {
                    TValue current;
                    if (this._dictionary.TryGetValue(key, out current)) {
                        var disposable = current as IDisposable;

                        if (disposable != null) {
                            disposable.Dispose();
                        }
                    }

                    this._dictionary[key] = value;
                }
                finally {
                    this._padlock.ExitWriteLock();
                }
            }
        }

        public IEnumerable<TKey> Keys {
            get {
                this._padlock.EnterReadLock();
                try {
                    return new List<TKey>(this._dictionary.Keys);
                }
                finally {
                    this._padlock.ExitReadLock();
                }
            }
        }

        #region IDisposable Members

        public void Dispose() {
            this._padlock.EnterWriteLock();

            try {
                var disposableItems = from item in this._dictionary.Values
                    where item is IDisposable
                    select item as IDisposable;

                foreach (var item in disposableItems) {
                    item.Dispose();
                }
            }
            finally {
                this._padlock.ExitWriteLock();
            }

            GC.SuppressFinalize(this);
        }

        #endregion

        public bool TryGetValue(TKey key, out TValue value) {
            this._padlock.EnterReadLock();
            try {
                return this._dictionary.TryGetValue(key, out value);
            }
            finally {
                this._padlock.ExitReadLock();
            }
        }

        public bool Remove(TKey key) {
            this._padlock.EnterWriteLock();
            try {
                return this._dictionary.Remove(key);
            }
            finally {
                this._padlock.ExitWriteLock();
            }
        }

        public void Clear() {
            this._padlock.EnterWriteLock();
            try {
                this._dictionary.Clear();
            }
            finally {
                this._padlock.ExitWriteLock();
            }
        }
    }
#else
#if TINYIOC_INTERNAL
    internal
#else
    public
#endif 
    class SafeDictionary<TKey, TValue> : IDisposable
    {
        private readonly object _Padlock = new object();
        private readonly Dictionary<TKey, TValue> _Dictionary = new Dictionary<TKey, TValue>();

        public TValue this[TKey key]
        {
            set
            {
                lock (_Padlock)
                {
                    TValue current;
                    if (_Dictionary.TryGetValue(key, out current))
                    {
                        var disposable = current as IDisposable;

                        if (disposable != null)
                            disposable.Dispose();
                    }

                    _Dictionary[key] = value;
                }
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_Padlock)
            {
                return _Dictionary.TryGetValue(key, out value);
            }
        }

        public bool Remove(TKey key)
        {
            lock (_Padlock)
            {
                return _Dictionary.Remove(key);
            }
        }

        public void Clear()
        {
            lock (_Padlock)
            {
                _Dictionary.Clear();
            }
        }

        public IEnumerable<TKey> Keys
        {
            get
            {
                return _Dictionary.Keys;
            }
        }
    #region IDisposable Members

        public void Dispose()
        {
            lock (_Padlock)
            {
                var disposableItems = from item in _Dictionary.Values
                                      where item is IDisposable
                                      select item as IDisposable;

                foreach (var item in disposableItems)
                {
                    item.Dispose();
                }
            }

            GC.SuppressFinalize(this);
        }

        #endregion
    }
#endif

    #endregion

    #region Extensions

#if TINYIOC_INTERNAL
    internal
#else
    public
#endif
        static class AssemblyExtensions {
        public static Type[] SafeGetTypes(this Assembly assembly) {
            Type[] assemblies;

            try {
                assemblies = assembly.GetTypes();
            }
            catch (FileNotFoundException) {
                assemblies = new Type[] {};
            }
            catch (NotSupportedException) {
                assemblies = new Type[] {};
            }
#if !NETFX_CORE
            catch (ReflectionTypeLoadException e) {
                assemblies = e.Types.Where(t => t != null).ToArray();
            }
#endif
            return assemblies;
        }
    }

#if TINYIOC_INTERNAL
    internal
#else
    public
#endif
        static class TypeExtensions {
        private static readonly SafeDictionary<GenericMethodCacheKey, MethodInfo> GenericMethodCache;

        static TypeExtensions() {
            GenericMethodCache = new SafeDictionary<GenericMethodCacheKey, MethodInfo>();
        }

        //#if NETFX_CORE
        //		/// <summary>
        //		/// Gets a generic method from a type given the method name, generic types and parameter types
        //		/// </summary>
        //		/// <param name="sourceType">Source type</param>
        //		/// <param name="methodName">Name of the method</param>
        //		/// <param name="genericTypes">Generic types to use to make the method generic</param>
        //		/// <param name="parameterTypes">Method parameters</param>
        //		/// <returns>MethodInfo or null if no matches found</returns>
        //		/// <exception cref="System.Reflection.AmbiguousMatchException"/>
        //		/// <exception cref="System.ArgumentException"/>
        //		public static MethodInfo GetGenericMethod(this Type sourceType, string methodName, Type[] genericTypes, Type[] parameterTypes)
        //		{
        //			MethodInfo method;
        //			var cacheKey = new GenericMethodCacheKey(sourceType, methodName, genericTypes, parameterTypes);

        //			// Shouldn't need any additional locking
        //			// we don't care if we do the method info generation
        //			// more than once before it gets cached.
        //			if (!_genericMethodCache.TryGetValue(cacheKey, out method))
        //			{
        //				method = GetMethod(sourceType, methodName, genericTypes, parameterTypes);
        //				_genericMethodCache[cacheKey] = method;
        //			}

        //			return method;
        //		}
        //#else
        /// <summary>
        ///     Gets a generic method from a type given the method name, binding flags, generic types and parameter types
        /// </summary>
        /// <param name="sourceType">Source type</param>
        /// <param name="bindingFlags">Binding flags</param>
        /// <param name="methodName">Name of the method</param>
        /// <param name="genericTypes">Generic types to use to make the method generic</param>
        /// <param name="parameterTypes">Method parameters</param>
        /// <returns>MethodInfo or null if no matches found</returns>
        /// <exception cref="System.Reflection.AmbiguousMatchException" />
        /// <exception cref="System.ArgumentException" />
        public static MethodInfo GetGenericMethod(this Type sourceType, BindingFlags bindingFlags, string methodName,
            Type[] genericTypes, Type[] parameterTypes) {
            MethodInfo method;
            var cacheKey = new GenericMethodCacheKey(sourceType, methodName, genericTypes, parameterTypes);

            // Shouldn't need any additional locking
            // we don't care if we do the method info generation
            // more than once before it gets cached.
            if (GenericMethodCache.TryGetValue(cacheKey, out method)) {
                return method;
            }
            method = GetMethod(sourceType, bindingFlags, methodName, genericTypes, parameterTypes);
            GenericMethodCache[cacheKey] = method;

            return method;
        }

        //#endif

#if NETFX_CORE
        private static MethodInfo GetMethod(Type sourceType, BindingFlags flags, string methodName, Type[] genericTypes, Type[] parameterTypes)
        {
            var methods =
                sourceType.GetMethods(flags).Where(
                    mi => string.Equals(methodName, mi.Name, StringComparison.Ordinal)).Where(
                        mi => mi.ContainsGenericParameters).Where(mi => mi.GetGenericArguments().Length == genericTypes.Length).
                    Where(mi => mi.GetParameters().Length == parameterTypes.Length).Select(
                        mi => mi.MakeGenericMethod(genericTypes)).Where(
                            mi => mi.GetParameters().Select(pi => pi.ParameterType).SequenceEqual(parameterTypes)).ToList();

            if (methods.Count > 1)
            {
                throw new AmbiguousMatchException();
            }

            return methods.FirstOrDefault();
        }
#else
        private static MethodInfo GetMethod(Type sourceType, BindingFlags bindingFlags, string methodName,
            Type[] genericTypes, Type[] parameterTypes) {
#if GETPARAMETERS_OPEN_GENERICS
            var methods =
                sourceType.GetMethods(bindingFlags).Where(
                    mi => string.Equals(methodName, mi.Name, StringComparison.Ordinal)).Where(
                        mi => mi.ContainsGenericParameters)
                    .Where(mi => mi.GetGenericArguments().Length == genericTypes.Length)
                    .
                    Where(mi => mi.GetParameters().Length == parameterTypes.Length).Select(
                        mi => mi.MakeGenericMethod(genericTypes)).Where(
                            mi => mi.GetParameters().Select(pi => pi.ParameterType).SequenceEqual(parameterTypes))
                    .ToList();
#else
            var validMethods =  from method in sourceType.GetMethods(bindingFlags)
                                where method.Name == methodName
                                where method.IsGenericMethod
                                where method.GetGenericArguments().Length == genericTypes.Length
                                let genericMethod = method.MakeGenericMethod(genericTypes)
                                where genericMethod.GetParameters().Count() == parameterTypes.Length
                                where genericMethod.GetParameters().Select(pi => pi.ParameterType).SequenceEqual(parameterTypes)
                                select genericMethod;

            var methods = validMethods.ToList();
#endif
            if (methods.Count > 1) {
                throw new AmbiguousMatchException();
            }

            return methods.FirstOrDefault();
        }
#endif

        private sealed class GenericMethodCacheKey {
            private readonly Type[] _genericTypes;
            private readonly int _hashCode;
            private readonly string _methodName;
            private readonly Type[] _parameterTypes;
            private readonly Type _sourceType;

            public GenericMethodCacheKey(Type sourceType, string methodName, Type[] genericTypes, Type[] parameterTypes) {
                this._sourceType = sourceType;
                this._methodName = methodName;
                this._genericTypes = genericTypes;
                this._parameterTypes = parameterTypes;
                this._hashCode = this.GenerateHashCode();
            }

            public override bool Equals(object obj) {
                var cacheKey = obj as GenericMethodCacheKey;
                if (cacheKey == null) {
                    return false;
                }

                if (this._sourceType != cacheKey._sourceType) {
                    return false;
                }

                if (!string.Equals(this._methodName, cacheKey._methodName, StringComparison.Ordinal)) {
                    return false;
                }

                if (this._genericTypes.Length != cacheKey._genericTypes.Length) {
                    return false;
                }

                if (this._parameterTypes.Length != cacheKey._parameterTypes.Length) {
                    return false;
                }

                if (this._genericTypes.Where((t, i) => t != cacheKey._genericTypes[i]).Any()) {
                    return false;
                }

                return !this._parameterTypes.Where((t, i) => t != cacheKey._parameterTypes[i]).Any();
            }

            public override int GetHashCode() {
                return this._hashCode;
            }

            private int GenerateHashCode() {
                unchecked {
                    var result = this._sourceType.GetHashCode();

                    result = (result*397) ^ this._methodName.GetHashCode();

                    result = this._genericTypes.Aggregate(result, (current, t) => (current*397) ^ t.GetHashCode());

                    return this._parameterTypes.Aggregate(result, (current, t) => (current*397) ^ t.GetHashCode());
                }
            }
        }
    }

    // @mbrit - 2012-05-22 - shim for ForEach call on List<T>...
#if NETFX_CORE
	internal static class ListExtender
	{
		internal static void ForEach<T>(this List<T> list, Action<T> callback)
		{
			foreach (T obj in list)
				callback(obj);
		}
	}
#endif

    #endregion

    #region TinyIoC Exception Types

#if TINYIOC_INTERNAL
    internal
#else
    public
#endif
        class TinyIoCResolutionException : Exception {
        private const string ErrorText = "Unable to resolve type: {0}";

        public TinyIoCResolutionException(Type type)
            : base(string.Format(ErrorText, type.FullName)) {}

        public TinyIoCResolutionException(Type type, Exception innerException)
            : base(string.Format(ErrorText, type.FullName), innerException) {}
    }

#if TINYIOC_INTERNAL
    internal
#else
    public
#endif
        class TinyIoCRegistrationTypeException : Exception {
        private const string RegisterErrorText =
            "Cannot register type {0} - abstract classes or interfaces are not valid implementation types for {1}.";

        public TinyIoCRegistrationTypeException(Type type, string factory)
            : base(string.Format(RegisterErrorText, type.FullName, factory)) {}

        public TinyIoCRegistrationTypeException(Type type, string factory, Exception innerException)
            : base(string.Format(RegisterErrorText, type.FullName, factory), innerException) {}
    }

#if TINYIOC_INTERNAL
    internal
#else
    public
#endif
        class TinyIoCRegistrationException : Exception {
        private const string ConvertErrorText = "Cannot convert current registration of {0} to {1}";
        private const string GenericConstraintErrorText = "Type {1} is not valid for a registration of type {0}";

        public TinyIoCRegistrationException(Type type, string method)
            : base(string.Format(ConvertErrorText, type.FullName, method)) {}

        public TinyIoCRegistrationException(Type type, string method, Exception innerException)
            : base(string.Format(ConvertErrorText, type.FullName, method), innerException) {}

        public TinyIoCRegistrationException(Type registerType, Type implementationType)
            : base(string.Format(GenericConstraintErrorText, registerType.FullName, implementationType.FullName)) {}

        public TinyIoCRegistrationException(Type registerType, Type implementationType, Exception innerException)
            : base(
                string.Format(GenericConstraintErrorText, registerType.FullName, implementationType.FullName),
                innerException) {}
    }

#if TINYIOC_INTERNAL
    internal
#else
    public
#endif
        class TinyIoCWeakReferenceException : Exception {
        private const string ErrorText = "Unable to instantiate {0} - referenced object has been reclaimed";

        public TinyIoCWeakReferenceException(Type type)
            : base(string.Format(ErrorText, type.FullName)) {}

        public TinyIoCWeakReferenceException(Type type, Exception innerException)
            : base(string.Format(ErrorText, type.FullName), innerException) {}
    }

#if TINYIOC_INTERNAL
    internal
#else
    public
#endif
        class TinyIoCConstructorResolutionException : Exception {
        private const string ErrorText = "Unable to resolve constructor for {0} using provided Expression.";

        public TinyIoCConstructorResolutionException(Type type)
            : base(string.Format(ErrorText, type.FullName)) {}

        public TinyIoCConstructorResolutionException(Type type, Exception innerException)
            : base(string.Format(ErrorText, type.FullName), innerException) {}

        public TinyIoCConstructorResolutionException(string message, Exception innerException)
            : base(message, innerException) {}

        public TinyIoCConstructorResolutionException(string message)
            : base(message) {}
    }

#if TINYIOC_INTERNAL
    internal
#else
    public
#endif
        class TinyIoCAutoRegistrationException : Exception {
        private const string ErrorText = "Duplicate implementation of type {0} found ({1}).";

        public TinyIoCAutoRegistrationException(Type registerType, IEnumerable<Type> types)
            : base(string.Format(ErrorText, registerType, GetTypesString(types))) {}

        public TinyIoCAutoRegistrationException(Type registerType, IEnumerable<Type> types, Exception innerException)
            : base(string.Format(ErrorText, registerType, GetTypesString(types)), innerException) {}

        private static string GetTypesString(IEnumerable<Type> types) {
            var typeNames = from type in types
                select type.FullName;

            return string.Join(",", typeNames.ToArray());
        }
    }

    #endregion

    #region Public Setup / Settings Classes

    /// <summary>
    ///     Name/Value pairs for specifying "user" parameters when resolving
    /// </summary>
#if TINYIOC_INTERNAL
    internal
#else
    public
#endif
        sealed class NamedParameterOverloads : Dictionary<string, object> {
        public static NamedParameterOverloads FromIDictionary(IDictionary<string, object> data) {
            return data as NamedParameterOverloads ?? new NamedParameterOverloads(data);
        }

        public NamedParameterOverloads() {}

        public NamedParameterOverloads(IDictionary<string, object> data)
            : base(data) {}

        private static readonly NamedParameterOverloads _default = new NamedParameterOverloads();

        public static NamedParameterOverloads Default {
            get { return _default; }
        }
    }

#if TINYIOC_INTERNAL
    internal
#else
    public
#endif
        enum UnregisteredResolutionActions {
        /// <summary>
        ///     Attempt to resolve type, even if the type isn't registered.
        ///     Registered types/options will always take precedence.
        /// </summary>
        AttemptResolve,

        /// <summary>
        ///     Fail resolution if type not explicitly registered
        /// </summary>
        Fail,

        /// <summary>
        ///     Attempt to resolve unregistered type if requested type is generic
        ///     and no registration exists for the specific generic parameters used.
        ///     Registered types/options will always take precedence.
        /// </summary>
        GenericsOnly
    }

#if TINYIOC_INTERNAL
    internal
#else
    public
#endif
        enum NamedResolutionFailureActions {
        AttemptUnnamedResolution,
        Fail
    }

#if TINYIOC_INTERNAL
    internal
#else
    public
#endif
        enum DuplicateImplementationActions {
        RegisterSingle,
        RegisterMultiple,
        Fail
    }

    /// <summary>
    ///     Resolution settings
    /// </summary>
#if TINYIOC_INTERNAL
    internal
#else
    public
#endif
        sealed class ResolveOptions {
        private static readonly ResolveOptions _Default = new ResolveOptions();

        private static readonly ResolveOptions _FailUnregisteredAndNameNotFound = new ResolveOptions {
            NamedResolutionFailureAction = NamedResolutionFailureActions.Fail,
            UnregisteredResolutionAction = UnregisteredResolutionActions.Fail
        };

        private static readonly ResolveOptions _FailUnregisteredOnly = new ResolveOptions {
            NamedResolutionFailureAction = NamedResolutionFailureActions.AttemptUnnamedResolution,
            UnregisteredResolutionAction = UnregisteredResolutionActions.Fail
        };

        private static readonly ResolveOptions _FailNameNotFoundOnly = new ResolveOptions {
            NamedResolutionFailureAction = NamedResolutionFailureActions.Fail,
            UnregisteredResolutionAction = UnregisteredResolutionActions.AttemptResolve
        };

        private UnregisteredResolutionActions _UnregisteredResolutionAction =
            UnregisteredResolutionActions.AttemptResolve;

        public UnregisteredResolutionActions UnregisteredResolutionAction {
            get { return this._UnregisteredResolutionAction; }
            set { this._UnregisteredResolutionAction = value; }
        }

        private NamedResolutionFailureActions _NamedResolutionFailureAction = NamedResolutionFailureActions.Fail;

        public NamedResolutionFailureActions NamedResolutionFailureAction {
            get { return this._NamedResolutionFailureAction; }
            set { this._NamedResolutionFailureAction = value; }
        }

        /// <summary>
        ///     Gets the default options (attempt resolution of unregistered types, fail on named resolution if name not found)
        /// </summary>
        public static ResolveOptions Default {
            get { return _Default; }
        }

        /// <summary>
        ///     Preconfigured option for attempting resolution of unregistered types and failing on named resolution if name not
        ///     found
        /// </summary>
        public static ResolveOptions FailNameNotFoundOnly {
            get { return _FailNameNotFoundOnly; }
        }

        /// <summary>
        ///     Preconfigured option for failing on resolving unregistered types and on named resolution if name not found
        /// </summary>
        public static ResolveOptions FailUnregisteredAndNameNotFound {
            get { return _FailUnregisteredAndNameNotFound; }
        }

        /// <summary>
        ///     Preconfigured option for failing on resolving unregistered types, but attempting unnamed resolution if name not
        ///     found
        /// </summary>
        public static ResolveOptions FailUnregisteredOnly {
            get { return _FailUnregisteredOnly; }
        }
    }

    #endregion

#if TINYIOC_INTERNAL
    internal
#else
    public
#endif
        sealed class TinyIoCContainer : IDisposable {
        #region Fake NETFX_CORE Classes

#if NETFX_CORE
        private sealed class MethodAccessException : Exception
        {
        }

        private sealed class AppDomain
        {
            public static AppDomain CurrentDomain { get; private set; }

            static AppDomain()
            {
                CurrentDomain = new AppDomain();
            }

			// @mbrit - 2012-05-30 - in WinRT, this should be done async...
            public async Task<List<Assembly>> GetAssembliesAsync()
            {
                var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;

                List<Assembly> assemblies = new List<Assembly>();

				var files = await folder.GetFilesAsync();

                foreach (StorageFile file in files)
                {
                    if (file.FileType == ".dll" || file.FileType == ".exe")
                    {
                        AssemblyName name = new AssemblyName() { Name = System.IO.Path.GetFileNameWithoutExtension(file.Name) };
						try
						{
							var asm = Assembly.Load(name);
							assemblies.Add(asm);
						}
						catch
						{
							// ignore exceptions here...
						}
                    }
                }

				return assemblies;
            }
        }
#endif

        #endregion

        #region "Fluent" API

        /// <summary>
        ///     Registration options for "fluent" API
        /// </summary>
        public sealed class RegisterOptions {
            private readonly TinyIoCContainer _container;
            private readonly TypeRegistration _registration;

            public RegisterOptions(TinyIoCContainer container, TypeRegistration registration) {
                this._container = container;
                this._registration = registration;
            }

            /// <summary>
            ///     Make registration a singleton (single instance) if possible
            /// </summary>
            /// <returns>RegisterOptions</returns>
            /// <exception cref="TinyIoCInstantiationTypeException"></exception>
            public RegisterOptions AsSingleton() {
                var currentFactory = this._container.GetCurrentFactory(this._registration);

                if (currentFactory == null) {
                    throw new TinyIoCRegistrationException(this._registration.Type, "singleton");
                }

                return this._container.AddUpdateRegistration(this._registration, currentFactory.SingletonVariant);
            }

            /// <summary>
            ///     Make registration multi-instance if possible
            /// </summary>
            /// <returns>RegisterOptions</returns>
            /// <exception cref="TinyIoCInstantiationTypeException"></exception>
            public RegisterOptions AsMultiInstance() {
                var currentFactory = this._container.GetCurrentFactory(this._registration);

                if (currentFactory == null) {
                    throw new TinyIoCRegistrationException(this._registration.Type, "multi-instance");
                }

                return this._container.AddUpdateRegistration(this._registration, currentFactory.MultiInstanceVariant);
            }

            /// <summary>
            ///     Make registration hold a weak reference if possible
            /// </summary>
            /// <returns>RegisterOptions</returns>
            /// <exception cref="TinyIoCInstantiationTypeException"></exception>
            public RegisterOptions WithWeakReference() {
                var currentFactory = this._container.GetCurrentFactory(this._registration);

                if (currentFactory == null) {
                    throw new TinyIoCRegistrationException(this._registration.Type, "weak reference");
                }

                return this._container.AddUpdateRegistration(this._registration, currentFactory.WeakReferenceVariant);
            }

            /// <summary>
            ///     Make registration hold a strong reference if possible
            /// </summary>
            /// <returns>RegisterOptions</returns>
            /// <exception cref="TinyIoCInstantiationTypeException"></exception>
            public RegisterOptions WithStrongReference() {
                var currentFactory = this._container.GetCurrentFactory(this._registration);

                if (currentFactory == null) {
                    throw new TinyIoCRegistrationException(this._registration.Type, "strong reference");
                }

                return this._container.AddUpdateRegistration(this._registration, currentFactory.StrongReferenceVariant);
            }

#if EXPRESSIONS
            public RegisterOptions UsingConstructor<TRegisterType>(Expression<Func<TRegisterType>> constructor) {
                var lambda = constructor as LambdaExpression;
                if (lambda == null) {
                    throw new TinyIoCConstructorResolutionException(typeof (TRegisterType));
                }

                var newExpression = lambda.Body as NewExpression;
                if (newExpression == null) {
                    throw new TinyIoCConstructorResolutionException(typeof (TRegisterType));
                }

                var constructorInfo = newExpression.Constructor;
                if (constructorInfo == null) {
                    throw new TinyIoCConstructorResolutionException(typeof (TRegisterType));
                }

                var currentFactory = this._container.GetCurrentFactory(this._registration);
                if (currentFactory == null) {
                    throw new TinyIoCConstructorResolutionException(typeof (TRegisterType));
                }

                currentFactory.SetConstructor(constructorInfo);

                return this;
            }
#endif

            /// <summary>
            ///     Switches to a custom lifetime manager factory if possible.
            ///     Usually used for RegisterOptions "To*" extension methods such as the ASP.Net per-request one.
            /// </summary>
            /// <param name="instance">RegisterOptions instance</param>
            /// <param name="lifetimeProvider">Custom lifetime manager</param>
            /// <param name="errorString">Error string to display if switch fails</param>
            /// <returns>RegisterOptions</returns>
            public static RegisterOptions ToCustomLifetimeManager(RegisterOptions instance,
                ITinyIoCObjectLifetimeProvider lifetimeProvider, string errorString) {
                if (instance == null) {
                    throw new ArgumentNullException("instance", "instance is null.");
                }

                if (lifetimeProvider == null) {
                    throw new ArgumentNullException("lifetimeProvider", "lifetimeProvider is null.");
                }

                if (string.IsNullOrEmpty(errorString)) {
                    throw new ArgumentException("errorString is null or empty.", "errorString");
                }

                var currentFactory = instance._container.GetCurrentFactory(instance._registration);

                if (currentFactory == null) {
                    throw new TinyIoCRegistrationException(instance._registration.Type, errorString);
                }

                return instance._container.AddUpdateRegistration(instance._registration,
                    currentFactory.GetCustomObjectLifetimeVariant(lifetimeProvider, errorString));
            }
        }

        /// <summary>
        ///     Registration options for "fluent" API when registering multiple implementations
        /// </summary>
        public sealed class MultiRegisterOptions {
            private IEnumerable<RegisterOptions> _registerOptions;

            /// <summary>
            ///     Initializes a new instance of the MultiRegisterOptions class.
            /// </summary>
            /// <param name="registerOptions">Registration options</param>
            public MultiRegisterOptions(IEnumerable<RegisterOptions> registerOptions) {
                this._registerOptions = registerOptions;
            }

            /// <summary>
            ///     Make registration a singleton (single instance) if possible
            /// </summary>
            /// <returns>RegisterOptions</returns>
            /// <exception cref="TinyIoCInstantiationTypeException"></exception>
            public MultiRegisterOptions AsSingleton() {
                this._registerOptions = this.ExecuteOnAllRegisterOptions(ro => ro.AsSingleton());
                return this;
            }

            /// <summary>
            ///     Make registration multi-instance if possible
            /// </summary>
            /// <returns>MultiRegisterOptions</returns>
            /// <exception cref="TinyIoCInstantiationTypeException"></exception>
            public MultiRegisterOptions AsMultiInstance() {
                this._registerOptions = this.ExecuteOnAllRegisterOptions(ro => ro.AsMultiInstance());
                return this;
            }

            /// <summary>
            ///     Switches to a custom lifetime manager factory if possible.
            ///     Usually used for RegisterOptions "To*" extension methods such as the ASP.Net per-request one.
            /// </summary>
            /// <param name="instance">MultiRegisterOptions instance</param>
            /// <param name="lifetimeProvider">Custom lifetime manager</param>
            /// <param name="errorString">Error string to display if switch fails</param>
            /// <returns>MultiRegisterOptions</returns>
            public static MultiRegisterOptions ToCustomLifetimeManager(
                MultiRegisterOptions instance,
                ITinyIoCObjectLifetimeProvider lifetimeProvider,
                string errorString) {
                if (instance == null) {
                    throw new ArgumentNullException("instance", "instance is null.");
                }

                if (lifetimeProvider == null) {
                    throw new ArgumentNullException("lifetimeProvider", "lifetimeProvider is null.");
                }

                if (string.IsNullOrEmpty(errorString)) {
                    throw new ArgumentException("errorString is null or empty.", "errorString");
                }

                instance._registerOptions =
                    instance.ExecuteOnAllRegisterOptions(
                        ro => RegisterOptions.ToCustomLifetimeManager(ro, lifetimeProvider, errorString));

                return instance;
            }

            private IEnumerable<RegisterOptions> ExecuteOnAllRegisterOptions(
                Func<RegisterOptions, RegisterOptions> action) {
                return this._registerOptions.Select(action).ToList();
            }
        }

        #endregion

        #region Public API

        #region Child Containers

        public TinyIoCContainer GetChildContainer() {
            return new TinyIoCContainer(this);
        }

        #endregion

        #region Registration

        /// <summary>
        ///     Attempt to automatically register all non-generic classes and interfaces in the current app domain.
        ///     If more than one class implements an interface then only one implementation will be registered
        ///     although no error will be thrown.
        /// </summary>
        public void AutoRegister() {
#if APPDOMAIN_GETASSEMBLIES
            this.AutoRegisterInternal(AppDomain.CurrentDomain.GetAssemblies().Where(a => !IsIgnoredAssembly(a)),
                DuplicateImplementationActions.RegisterSingle, null);
#else
            AutoRegisterInternal(new Assembly[] {this.GetType().Assembly()}, true, null);
#endif
        }

        /// <summary>
        ///     Attempt to automatically register all non-generic classes and interfaces in the current app domain.
        ///     Types will only be registered if they pass the supplied registration predicate.
        ///     If more than one class implements an interface then only one implementation will be registered
        ///     although no error will be thrown.
        /// </summary>
        /// <param name="registrationPredicate">Predicate to determine if a particular type should be registered</param>
        public void AutoRegister(Func<Type, bool> registrationPredicate) {
#if APPDOMAIN_GETASSEMBLIES
            this.AutoRegisterInternal(AppDomain.CurrentDomain.GetAssemblies().Where(a => !IsIgnoredAssembly(a)),
                DuplicateImplementationActions.RegisterSingle, registrationPredicate);
#else
            AutoRegisterInternal(new Assembly[] { this.GetType().Assembly()}, true, registrationPredicate);
#endif
        }

        /// <summary>
        ///     Attempt to automatically register all non-generic classes and interfaces in the current app domain.
        /// </summary>
        /// <param name="duplicateAction">
        ///     What action to take when encountering duplicate implementations of an interface/base
        ///     class.
        /// </param>
        /// <exception cref="TinyIoCAutoRegistrationException" />
        public void AutoRegister(DuplicateImplementationActions duplicateAction) {
#if APPDOMAIN_GETASSEMBLIES
            this.AutoRegisterInternal(AppDomain.CurrentDomain.GetAssemblies().Where(a => !IsIgnoredAssembly(a)),
                duplicateAction, null);
#else
            AutoRegisterInternal(new Assembly[] { this.GetType().Assembly() }, ignoreDuplicateImplementations, null);
#endif
        }

        /// <summary>
        ///     Attempt to automatically register all non-generic classes and interfaces in the current app domain.
        ///     Types will only be registered if they pass the supplied registration predicate.
        /// </summary>
        /// <param name="duplicateAction">
        ///     What action to take when encountering duplicate implementations of an interface/base
        ///     class.
        /// </param>
        /// <param name="registrationPredicate">Predicate to determine if a particular type should be registered</param>
        /// <exception cref="TinyIoCAutoRegistrationException" />
        public void AutoRegister(DuplicateImplementationActions duplicateAction, Func<Type, bool> registrationPredicate) {
#if APPDOMAIN_GETASSEMBLIES
            this.AutoRegisterInternal(AppDomain.CurrentDomain.GetAssemblies().Where(a => !IsIgnoredAssembly(a)),
                duplicateAction, registrationPredicate);
#else
            AutoRegisterInternal(new Assembly[] { this.GetType().Assembly() }, ignoreDuplicateImplementations, registrationPredicate);
#endif
        }

        /// <summary>
        ///     Attempt to automatically register all non-generic classes and interfaces in the specified assemblies
        ///     If more than one class implements an interface then only one implementation will be registered
        ///     although no error will be thrown.
        /// </summary>
        /// <param name="assemblies">Assemblies to process</param>
        public void AutoRegister(IEnumerable<Assembly> assemblies) {
            this.AutoRegisterInternal(assemblies, DuplicateImplementationActions.RegisterSingle, null);
        }

        /// <summary>
        ///     Attempt to automatically register all non-generic classes and interfaces in the specified assemblies
        ///     Types will only be registered if they pass the supplied registration predicate.
        ///     If more than one class implements an interface then only one implementation will be registered
        ///     although no error will be thrown.
        /// </summary>
        /// <param name="assemblies">Assemblies to process</param>
        /// <param name="registrationPredicate">Predicate to determine if a particular type should be registered</param>
        public void AutoRegister(IEnumerable<Assembly> assemblies, Func<Type, bool> registrationPredicate) {
            this.AutoRegisterInternal(assemblies, DuplicateImplementationActions.RegisterSingle, registrationPredicate);
        }

        /// <summary>
        ///     Attempt to automatically register all non-generic classes and interfaces in the specified assemblies
        /// </summary>
        /// <param name="assemblies">Assemblies to process</param>
        /// <param name="duplicateAction">
        ///     What action to take when encountering duplicate implementations of an interface/base
        ///     class.
        /// </param>
        /// <exception cref="TinyIoCAutoRegistrationException" />
        public void AutoRegister(IEnumerable<Assembly> assemblies, DuplicateImplementationActions duplicateAction) {
            this.AutoRegisterInternal(assemblies, duplicateAction, null);
        }

        /// <summary>
        ///     Attempt to automatically register all non-generic classes and interfaces in the specified assemblies
        ///     Types will only be registered if they pass the supplied registration predicate.
        /// </summary>
        /// <param name="assemblies">Assemblies to process</param>
        /// <param name="duplicateAction">
        ///     What action to take when encountering duplicate implementations of an interface/base
        ///     class.
        /// </param>
        /// <param name="registrationPredicate">Predicate to determine if a particular type should be registered</param>
        /// <exception cref="TinyIoCAutoRegistrationException" />
        public void AutoRegister(IEnumerable<Assembly> assemblies, DuplicateImplementationActions duplicateAction,
            Func<Type, bool> registrationPredicate) {
            this.AutoRegisterInternal(assemblies, duplicateAction, registrationPredicate);
        }

        /// <summary>
        ///     Creates/replaces a container class registration with default options.
        /// </summary>
        /// <param name="registerType">Type to register</param>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register(Type registerType) {
            return this.RegisterInternal(registerType, string.Empty,
                GetDefaultObjectFactory(registerType, registerType));
        }

        /// <summary>
        ///     Creates/replaces a named container class registration with default options.
        /// </summary>
        /// <param name="registerType">Type to register</param>
        /// <param name="name">Name of registration</param>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register(Type registerType, string name) {
            return this.RegisterInternal(registerType, name, GetDefaultObjectFactory(registerType, registerType));
        }

        /// <summary>
        ///     Creates/replaces a container class registration with a given implementation and default options.
        /// </summary>
        /// <param name="registerType">Type to register</param>
        /// <param name="registerImplementation">Type to instantiate that implements RegisterType</param>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register(Type registerType, Type registerImplementation) {
            return this.RegisterInternal(registerType, string.Empty,
                GetDefaultObjectFactory(registerType, registerImplementation));
        }

        /// <summary>
        ///     Creates/replaces a named container class registration with a given implementation and default options.
        /// </summary>
        /// <param name="registerType">Type to register</param>
        /// <param name="registerImplementation">Type to instantiate that implements RegisterType</param>
        /// <param name="name">Name of registration</param>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register(Type registerType, Type registerImplementation, string name) {
            return this.RegisterInternal(registerType, name,
                GetDefaultObjectFactory(registerType, registerImplementation));
        }

        /// <summary>
        ///     Creates/replaces a container class registration with a specific, strong referenced, instance.
        /// </summary>
        /// <param name="registerType">Type to register</param>
        /// <param name="instance">Instance of RegisterType to register</param>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register(Type registerType, object instance) {
            return this.RegisterInternal(registerType, string.Empty,
                new InstanceFactory(registerType, registerType, instance));
        }

        /// <summary>
        ///     Creates/replaces a named container class registration with a specific, strong referenced, instance.
        /// </summary>
        /// <param name="registerType">Type to register</param>
        /// <param name="instance">Instance of RegisterType to register</param>
        /// <param name="name">Name of registration</param>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register(Type registerType, object instance, string name) {
            return this.RegisterInternal(registerType, name, new InstanceFactory(registerType, registerType, instance));
        }

        /// <summary>
        ///     Creates/replaces a container class registration with a specific, strong referenced, instance.
        /// </summary>
        /// <param name="registerType">Type to register</param>
        /// <param name="registerImplementation">Type of instance to register that implements RegisterType</param>
        /// <param name="instance">Instance of RegisterImplementation to register</param>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register(Type registerType, Type registerImplementation, object instance) {
            return this.RegisterInternal(registerType, string.Empty,
                new InstanceFactory(registerType, registerImplementation, instance));
        }

        /// <summary>
        ///     Creates/replaces a named container class registration with a specific, strong referenced, instance.
        /// </summary>
        /// <param name="registerType">Type to register</param>
        /// <param name="registerImplementation">Type of instance to register that implements RegisterType</param>
        /// <param name="instance">Instance of RegisterImplementation to register</param>
        /// <param name="name">Name of registration</param>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register(Type registerType, Type registerImplementation, object instance, string name) {
            return this.RegisterInternal(registerType, name,
                new InstanceFactory(registerType, registerImplementation, instance));
        }

        /// <summary>
        ///     Creates/replaces a container class registration with a user specified factory
        /// </summary>
        /// <param name="registerType">Type to register</param>
        /// <param name="factory">Factory/lambda that returns an instance of RegisterType</param>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register(Type registerType,
            Func<TinyIoCContainer, NamedParameterOverloads, object> factory) {
            return this.RegisterInternal(registerType, string.Empty, new DelegateFactory(registerType, factory));
        }

        /// <summary>
        ///     Creates/replaces a container class registration with a user specified factory
        /// </summary>
        /// <param name="registerType">Type to register</param>
        /// <param name="factory">Factory/lambda that returns an instance of RegisterType</param>
        /// <param name="name">Name of registation</param>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register(Type registerType,
            Func<TinyIoCContainer, NamedParameterOverloads, object> factory, string name) {
            return this.RegisterInternal(registerType, name, new DelegateFactory(registerType, factory));
        }

        /// <summary>
        ///     Creates/replaces a container class registration with default options.
        /// </summary>
        /// <typeparam name="RegisterImplementation">Type to register</typeparam>
        /// <typeparam name="TRegisterType"></typeparam>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register<TRegisterType>()
            where TRegisterType : class {
            return this.Register(typeof (TRegisterType));
        }

        /// <summary>
        ///     Creates/replaces a named container class registration with default options.
        /// </summary>
        /// <typeparam name="RegisterImplementation">Type to register</typeparam>
        /// <typeparam name="TRegisterType"></typeparam>
        /// <param name="name">Name of registration</param>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register<TRegisterType>(string name)
            where TRegisterType : class {
            return this.Register(typeof (TRegisterType), name);
        }

        /// <summary>
        ///     Creates/replaces a container class registration with a given implementation and default options.
        /// </summary>
        /// <typeparam name="TRegisterType">Type to register</typeparam>
        /// <typeparam name="TRegisterImplementation">Type to instantiate that implements RegisterType</typeparam>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register<TRegisterType, TRegisterImplementation>()
            where TRegisterType : class
            where TRegisterImplementation : class, TRegisterType {
            return this.Register(typeof (TRegisterType), typeof (TRegisterImplementation));
        }

        /// <summary>
        ///     Creates/replaces a named container class registration with a given implementation and default options.
        /// </summary>
        /// <typeparam name="TRegisterType">Type to register</typeparam>
        /// <typeparam name="TRegisterImplementation">Type to instantiate that implements RegisterType</typeparam>
        /// <param name="name">Name of registration</param>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register<TRegisterType, TRegisterImplementation>(string name)
            where TRegisterType : class
            where TRegisterImplementation : class, TRegisterType {
            return this.Register(typeof (TRegisterType), typeof (TRegisterImplementation), name);
        }

        /// <summary>
        ///     Creates/replaces a container class registration with a specific, strong referenced, instance.
        /// </summary>
        /// <typeparam name="TRegisterType">Type to register</typeparam>
        /// <param name="instance">Instance of RegisterType to register</param>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register<TRegisterType>(TRegisterType instance)
            where TRegisterType : class {
            return this.Register(typeof (TRegisterType), instance);
        }

        /// <summary>
        ///     Creates/replaces a named container class registration with a specific, strong referenced, instance.
        /// </summary>
        /// <typeparam name="TRegisterType">Type to register</typeparam>
        /// <param name="instance">Instance of RegisterType to register</param>
        /// <param name="name">Name of registration</param>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register<TRegisterType>(TRegisterType instance, string name)
            where TRegisterType : class {
            return this.Register(typeof (TRegisterType), instance, name);
        }

        /// <summary>
        ///     Creates/replaces a container class registration with a specific, strong referenced, instance.
        /// </summary>
        /// <typeparam name="TRegisterType">Type to register</typeparam>
        /// <typeparam name="TRegisterImplementation">Type of instance to register that implements RegisterType</typeparam>
        /// <param name="instance">Instance of RegisterImplementation to register</param>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register<TRegisterType, TRegisterImplementation>(TRegisterImplementation instance)
            where TRegisterType : class
            where TRegisterImplementation : class, TRegisterType {
            return this.Register(typeof (TRegisterType), typeof (TRegisterImplementation), instance);
        }

        /// <summary>
        ///     Creates/replaces a named container class registration with a specific, strong referenced, instance.
        /// </summary>
        /// <typeparam name="TRegisterType">Type to register</typeparam>
        /// <typeparam name="TRegisterImplementation">Type of instance to register that implements RegisterType</typeparam>
        /// <param name="instance">Instance of RegisterImplementation to register</param>
        /// <param name="name">Name of registration</param>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register<TRegisterType, TRegisterImplementation>(TRegisterImplementation instance,
            string name)
            where TRegisterType : class
            where TRegisterImplementation : class, TRegisterType {
            return this.Register(typeof (TRegisterType), typeof (TRegisterImplementation), instance, name);
        }

        /// <summary>
        ///     Creates/replaces a container class registration with a user specified factory
        /// </summary>
        /// <typeparam name="TRegisterType">Type to register</typeparam>
        /// <param name="factory">Factory/lambda that returns an instance of RegisterType</param>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register<TRegisterType>(
            Func<TinyIoCContainer, NamedParameterOverloads, TRegisterType> factory)
            where TRegisterType : class {
            if (factory == null) {
                throw new ArgumentNullException("factory");
            }

            return this.Register(typeof (TRegisterType), factory);
        }

        /// <summary>
        ///     Creates/replaces a named container class registration with a user specified factory
        /// </summary>
        /// <typeparam name="TRegisterType">Type to register</typeparam>
        /// <param name="factory">Factory/lambda that returns an instance of RegisterType</param>
        /// <param name="name">Name of registation</param>
        /// <returns>RegisterOptions for fluent API</returns>
        public RegisterOptions Register<TRegisterType>(
            Func<TinyIoCContainer, NamedParameterOverloads, TRegisterType> factory, string name)
            where TRegisterType : class {
            if (factory == null) {
                throw new ArgumentNullException("factory");
            }

            return this.Register(typeof (TRegisterType), factory, name);
        }

        /// <summary>
        ///     Register multiple implementations of a type.
        ///     Internally this registers each implementation using the full name of the class as its registration name.
        /// </summary>
        /// <typeparam name="TRegisterType">Type that each implementation implements</typeparam>
        /// <param name="implementationTypes">Types that implement RegisterType</param>
        /// <returns>MultiRegisterOptions for the fluent API</returns>
        public MultiRegisterOptions RegisterMultiple<TRegisterType>(IEnumerable<Type> implementationTypes) {
            return this.RegisterMultiple(typeof (TRegisterType), implementationTypes);
        }

        /// <summary>
        ///     Register multiple implementations of a type.
        ///     Internally this registers each implementation using the full name of the class as its registration name.
        /// </summary>
        /// <param name="registrationType">Type that each implementation implements</param>
        /// <param name="implementationTypes">Types that implement RegisterType</param>
        /// <returns>MultiRegisterOptions for the fluent API</returns>
        public MultiRegisterOptions RegisterMultiple(Type registrationType, IEnumerable<Type> implementationTypes) {
            if (implementationTypes == null) {
                throw new ArgumentNullException("implementationTypes", "types is null.");
            }

            var enumerable = implementationTypes as Type[] ?? implementationTypes.ToArray();
            foreach (var type in enumerable.Where(type => !registrationType.IsAssignableFrom(type))) {
                //#endif
                throw new ArgumentException(string.Format("types: The type {0} is not assignable from {1}",
                    registrationType.FullName, type.FullName));
            }

            if (enumerable.Count() != enumerable.Distinct().Count()) {
                var queryForDuplicatedTypes = from i in enumerable
                    group i by i
                    into j
                    where j.Count() > 1
                    select j.Key.FullName;

                var fullNamesOfDuplicatedTypes = string.Join(",\n", queryForDuplicatedTypes.ToArray());
                var multipleRegMessage =
                    string.Format(
                        "types: The same implementation type cannot be specified multiple times for {0}\n\n{1}",
                        registrationType.FullName, fullNamesOfDuplicatedTypes);
                throw new ArgumentException(multipleRegMessage);
            }

            var registerOptions = enumerable.Select(type => this.Register(registrationType, type, type.FullName)).ToList();

            return new MultiRegisterOptions(registerOptions);
        }

        #endregion

        #region Resolution

        /// <summary>
        ///     Attempts to resolve a type using default options.
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <returns>Instance of type</returns>
        /// <exception cref="TinyIoCResolutionException">Unable to resolve the type.</exception>
        public object Resolve(Type resolveType) {
            return this.ResolveInternal(new TypeRegistration(resolveType), NamedParameterOverloads.Default,
                ResolveOptions.Default);
        }

        /// <summary>
        ///     Attempts to resolve a type using specified options.
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="options">Resolution options</param>
        /// <returns>Instance of type</returns>
        /// <exception cref="TinyIoCResolutionException">Unable to resolve the type.</exception>
        public object Resolve(Type resolveType, ResolveOptions options) {
            return this.ResolveInternal(new TypeRegistration(resolveType), NamedParameterOverloads.Default, options);
        }

        /// <summary>
        ///     Attempts to resolve a type using default options and the supplied name.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="name">Name of registration</param>
        /// <returns>Instance of type</returns>
        /// <exception cref="TinyIoCResolutionException">Unable to resolve the type.</exception>
        public object Resolve(Type resolveType, string name) {
            return this.ResolveInternal(new TypeRegistration(resolveType, name), NamedParameterOverloads.Default,
                ResolveOptions.Default);
        }

        /// <summary>
        ///     Attempts to resolve a type using supplied options and  name.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="name">Name of registration</param>
        /// <param name="options">Resolution options</param>
        /// <returns>Instance of type</returns>
        /// <exception cref="TinyIoCResolutionException">Unable to resolve the type.</exception>
        public object Resolve(Type resolveType, string name, ResolveOptions options) {
            return this.ResolveInternal(new TypeRegistration(resolveType, name), NamedParameterOverloads.Default,
                options);
        }

        /// <summary>
        ///     Attempts to resolve a type using default options and the supplied constructor parameters.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="parameters">User specified constructor parameters</param>
        /// <returns>Instance of type</returns>
        /// <exception cref="TinyIoCResolutionException">Unable to resolve the type.</exception>
        public object Resolve(Type resolveType, NamedParameterOverloads parameters) {
            return this.ResolveInternal(new TypeRegistration(resolveType), parameters, ResolveOptions.Default);
        }

        /// <summary>
        ///     Attempts to resolve a type using specified options and the supplied constructor parameters.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="parameters">User specified constructor parameters</param>
        /// <param name="options">Resolution options</param>
        /// <returns>Instance of type</returns>
        /// <exception cref="TinyIoCResolutionException">Unable to resolve the type.</exception>
        public object Resolve(Type resolveType, NamedParameterOverloads parameters, ResolveOptions options) {
            return this.ResolveInternal(new TypeRegistration(resolveType), parameters, options);
        }

        /// <summary>
        ///     Attempts to resolve a type using default options and the supplied constructor parameters and name.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="parameters">User specified constructor parameters</param>
        /// <param name="name">Name of registration</param>
        /// <returns>Instance of type</returns>
        /// <exception cref="TinyIoCResolutionException">Unable to resolve the type.</exception>
        public object Resolve(Type resolveType, string name, NamedParameterOverloads parameters) {
            return this.ResolveInternal(new TypeRegistration(resolveType, name), parameters, ResolveOptions.Default);
        }

        /// <summary>
        ///     Attempts to resolve a named type using specified options and the supplied constructor parameters.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="name">Name of registration</param>
        /// <param name="parameters">User specified constructor parameters</param>
        /// <param name="options">Resolution options</param>
        /// <returns>Instance of type</returns>
        /// <exception cref="TinyIoCResolutionException">Unable to resolve the type.</exception>
        public object Resolve(Type resolveType, string name, NamedParameterOverloads parameters, ResolveOptions options) {
            return this.ResolveInternal(new TypeRegistration(resolveType, name), parameters, options);
        }

        /// <summary>
        ///     Attempts to resolve a type using default options.
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <returns>Instance of type</returns>
        /// <exception cref="TinyIoCResolutionException">Unable to resolve the type.</exception>
        public TResolveType Resolve<TResolveType>()
            where TResolveType : class {
            return (TResolveType) this.Resolve(typeof (TResolveType));
        }

        /// <summary>
        ///     Attempts to resolve a type using specified options.
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="options">Resolution options</param>
        /// <returns>Instance of type</returns>
        /// <exception cref="TinyIoCResolutionException">Unable to resolve the type.</exception>
        public TResolveType Resolve<TResolveType>(ResolveOptions options)
            where TResolveType : class {
            return (TResolveType) this.Resolve(typeof (TResolveType), options);
        }

        /// <summary>
        ///     Attempts to resolve a type using default options and the supplied name.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="name">Name of registration</param>
        /// <returns>Instance of type</returns>
        /// <exception cref="TinyIoCResolutionException">Unable to resolve the type.</exception>
        public TResolveType Resolve<TResolveType>(string name)
            where TResolveType : class {
            return (TResolveType) this.Resolve(typeof (TResolveType), name);
        }

        /// <summary>
        ///     Attempts to resolve a type using supplied options and  name.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="name">Name of registration</param>
        /// <param name="options">Resolution options</param>
        /// <returns>Instance of type</returns>
        /// <exception cref="TinyIoCResolutionException">Unable to resolve the type.</exception>
        public TResolveType Resolve<TResolveType>(string name, ResolveOptions options)
            where TResolveType : class {
            return (TResolveType) this.Resolve(typeof (TResolveType), name, options);
        }

        /// <summary>
        ///     Attempts to resolve a type using default options and the supplied constructor parameters.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="parameters">User specified constructor parameters</param>
        /// <returns>Instance of type</returns>
        /// <exception cref="TinyIoCResolutionException">Unable to resolve the type.</exception>
        public TResolveType Resolve<TResolveType>(NamedParameterOverloads parameters)
            where TResolveType : class {
            return (TResolveType) this.Resolve(typeof (TResolveType), parameters);
        }

        /// <summary>
        ///     Attempts to resolve a type using specified options and the supplied constructor parameters.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="parameters">User specified constructor parameters</param>
        /// <param name="options">Resolution options</param>
        /// <returns>Instance of type</returns>
        /// <exception cref="TinyIoCResolutionException">Unable to resolve the type.</exception>
        public TResolveType Resolve<TResolveType>(NamedParameterOverloads parameters, ResolveOptions options)
            where TResolveType : class {
            return (TResolveType) this.Resolve(typeof (TResolveType), parameters, options);
        }

        /// <summary>
        ///     Attempts to resolve a type using default options and the supplied constructor parameters and name.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="parameters">User specified constructor parameters</param>
        /// <param name="name">Name of registration</param>
        /// <returns>Instance of type</returns>
        /// <exception cref="TinyIoCResolutionException">Unable to resolve the type.</exception>
        public TResolveType Resolve<TResolveType>(string name, NamedParameterOverloads parameters)
            where TResolveType : class {
            return (TResolveType) this.Resolve(typeof (TResolveType), name, parameters);
        }

        /// <summary>
        ///     Attempts to resolve a named type using specified options and the supplied constructor parameters.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="name">Name of registration</param>
        /// <param name="parameters">User specified constructor parameters</param>
        /// <param name="options">Resolution options</param>
        /// <returns>Instance of type</returns>
        /// <exception cref="TinyIoCResolutionException">Unable to resolve the type.</exception>
        public TResolveType Resolve<TResolveType>(string name, NamedParameterOverloads parameters, ResolveOptions options)
            where TResolveType : class {
            return (TResolveType) this.Resolve(typeof (TResolveType), name, parameters, options);
        }

        /// <summary>
        ///     Attempts to predict whether a given type can be resolved with default options.
        ///     Note: Resolution may still fail if user defined factory registations fail to construct objects when called.
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <returns>Bool indicating whether the type can be resolved</returns>
        public bool CanResolve(Type resolveType) {
            return this.CanResolveInternal(new TypeRegistration(resolveType), NamedParameterOverloads.Default,
                ResolveOptions.Default);
        }

        /// <summary>
        ///     Attempts to predict whether a given named type can be resolved with default options.
        ///     Note: Resolution may still fail if user defined factory registations fail to construct objects when called.
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="name">Name of registration</param>
        /// <returns>Bool indicating whether the type can be resolved</returns>
        private bool CanResolve(Type resolveType, string name) {
            return this.CanResolveInternal(new TypeRegistration(resolveType, name), NamedParameterOverloads.Default,
                ResolveOptions.Default);
        }

        /// <summary>
        ///     Attempts to predict whether a given type can be resolved with the specified options.
        ///     Note: Resolution may still fail if user defined factory registations fail to construct objects when called.
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="options">Resolution options</param>
        /// <returns>Bool indicating whether the type can be resolved</returns>
        public bool CanResolve(Type resolveType, ResolveOptions options) {
            return this.CanResolveInternal(new TypeRegistration(resolveType), NamedParameterOverloads.Default, options);
        }

        /// <summary>
        ///     Attempts to predict whether a given named type can be resolved with the specified options.
        ///     Note: Resolution may still fail if user defined factory registations fail to construct objects when called.
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="name">Name of registration</param>
        /// <param name="options">Resolution options</param>
        /// <returns>Bool indicating whether the type can be resolved</returns>
        public bool CanResolve(Type resolveType, string name, ResolveOptions options) {
            return this.CanResolveInternal(new TypeRegistration(resolveType, name), NamedParameterOverloads.Default,
                options);
        }

        /// <summary>
        ///     Attempts to predict whether a given type can be resolved with the supplied constructor parameters and default
        ///     options.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        ///     Note: Resolution may still fail if user defined factory registations fail to construct objects when called.
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="parameters">User supplied named parameter overloads</param>
        /// <returns>Bool indicating whether the type can be resolved</returns>
        public bool CanResolve(Type resolveType, NamedParameterOverloads parameters) {
            return this.CanResolveInternal(new TypeRegistration(resolveType), parameters, ResolveOptions.Default);
        }

        /// <summary>
        ///     Attempts to predict whether a given named type can be resolved with the supplied constructor parameters and default
        ///     options.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        ///     Note: Resolution may still fail if user defined factory registations fail to construct objects when called.
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="name">Name of registration</param>
        /// <param name="parameters">User supplied named parameter overloads</param>
        /// <returns>Bool indicating whether the type can be resolved</returns>
        public bool CanResolve(Type resolveType, string name, NamedParameterOverloads parameters) {
            return this.CanResolveInternal(new TypeRegistration(resolveType, name), parameters, ResolveOptions.Default);
        }

        /// <summary>
        ///     Attempts to predict whether a given type can be resolved with the supplied constructor parameters options.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        ///     Note: Resolution may still fail if user defined factory registations fail to construct objects when called.
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="parameters">User supplied named parameter overloads</param>
        /// <param name="options">Resolution options</param>
        /// <returns>Bool indicating whether the type can be resolved</returns>
        public bool CanResolve(Type resolveType, NamedParameterOverloads parameters, ResolveOptions options) {
            return this.CanResolveInternal(new TypeRegistration(resolveType), parameters, options);
        }

        /// <summary>
        ///     Attempts to predict whether a given named type can be resolved with the supplied constructor parameters options.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        ///     Note: Resolution may still fail if user defined factory registations fail to construct objects when called.
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="name">Name of registration</param>
        /// <param name="parameters">User supplied named parameter overloads</param>
        /// <param name="options">Resolution options</param>
        /// <returns>Bool indicating whether the type can be resolved</returns>
        public bool CanResolve(Type resolveType, string name, NamedParameterOverloads parameters, ResolveOptions options) {
            return this.CanResolveInternal(new TypeRegistration(resolveType, name), parameters, options);
        }

        /// <summary>
        ///     Attempts to predict whether a given type can be resolved with default options.
        ///     Note: Resolution may still fail if user defined factory registations fail to construct objects when called.
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <returns>Bool indicating whether the type can be resolved</returns>
        public bool CanResolve<TResolveType>()
            where TResolveType : class {
            return this.CanResolve(typeof (TResolveType));
        }

        /// <summary>
        ///     Attempts to predict whether a given named type can be resolved with default options.
        ///     Note: Resolution may still fail if user defined factory registations fail to construct objects when called.
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <returns>Bool indicating whether the type can be resolved</returns>
        public bool CanResolve<TResolveType>(string name)
            where TResolveType : class {
            return this.CanResolve(typeof (TResolveType), name);
        }

        /// <summary>
        ///     Attempts to predict whether a given type can be resolved with the specified options.
        ///     Note: Resolution may still fail if user defined factory registations fail to construct objects when called.
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="options">Resolution options</param>
        /// <returns>Bool indicating whether the type can be resolved</returns>
        public bool CanResolve<TResolveType>(ResolveOptions options)
            where TResolveType : class {
            return this.CanResolve(typeof (TResolveType), options);
        }

        /// <summary>
        ///     Attempts to predict whether a given named type can be resolved with the specified options.
        ///     Note: Resolution may still fail if user defined factory registations fail to construct objects when called.
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="name">Name of registration</param>
        /// <param name="options">Resolution options</param>
        /// <returns>Bool indicating whether the type can be resolved</returns>
        public bool CanResolve<TResolveType>(string name, ResolveOptions options)
            where TResolveType : class {
            return this.CanResolve(typeof (TResolveType), name, options);
        }

        /// <summary>
        ///     Attempts to predict whether a given type can be resolved with the supplied constructor parameters and default
        ///     options.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        ///     Note: Resolution may still fail if user defined factory registations fail to construct objects when called.
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="parameters">User supplied named parameter overloads</param>
        /// <returns>Bool indicating whether the type can be resolved</returns>
        public bool CanResolve<TResolveType>(NamedParameterOverloads parameters)
            where TResolveType : class {
            return this.CanResolve(typeof (TResolveType), parameters);
        }

        /// <summary>
        ///     Attempts to predict whether a given named type can be resolved with the supplied constructor parameters and default
        ///     options.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        ///     Note: Resolution may still fail if user defined factory registations fail to construct objects when called.
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="name">Name of registration</param>
        /// <param name="parameters">User supplied named parameter overloads</param>
        /// <returns>Bool indicating whether the type can be resolved</returns>
        public bool CanResolve<TResolveType>(string name, NamedParameterOverloads parameters)
            where TResolveType : class {
            return this.CanResolve(typeof (TResolveType), name, parameters);
        }

        /// <summary>
        ///     Attempts to predict whether a given type can be resolved with the supplied constructor parameters options.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        ///     Note: Resolution may still fail if user defined factory registations fail to construct objects when called.
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="parameters">User supplied named parameter overloads</param>
        /// <param name="options">Resolution options</param>
        /// <returns>Bool indicating whether the type can be resolved</returns>
        public bool CanResolve<TResolveType>(NamedParameterOverloads parameters, ResolveOptions options)
            where TResolveType : class {
            return this.CanResolve(typeof (TResolveType), parameters, options);
        }

        /// <summary>
        ///     Attempts to predict whether a given named type can be resolved with the supplied constructor parameters options.
        ///     Parameters are used in conjunction with normal container resolution to find the most suitable constructor (if one
        ///     exists).
        ///     All user supplied parameters must exist in at least one resolvable constructor of RegisterType or resolution will
        ///     fail.
        ///     Note: Resolution may still fail if user defined factory registations fail to construct objects when called.
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="name">Name of registration</param>
        /// <param name="parameters">User supplied named parameter overloads</param>
        /// <param name="options">Resolution options</param>
        /// <returns>Bool indicating whether the type can be resolved</returns>
        public bool CanResolve<TResolveType>(string name, NamedParameterOverloads parameters, ResolveOptions options)
            where TResolveType : class {
            return this.CanResolve(typeof (TResolveType), name, parameters, options);
        }

        /// <summary>
        ///     Attemps to resolve a type using the default options
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="resolvedType">Resolved type or default if resolve fails</param>
        /// <returns>True if resolved sucessfully, false otherwise</returns>
        public bool TryResolve(Type resolveType, out object resolvedType) {
            try {
                resolvedType = this.Resolve(resolveType);
                return true;
            }
            catch (TinyIoCResolutionException) {
                resolvedType = null;
                return false;
            }
        }

        /// <summary>
        ///     Attemps to resolve a type using the given options
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="options">Resolution options</param>
        /// <param name="resolvedType">Resolved type or default if resolve fails</param>
        /// <returns>True if resolved sucessfully, false otherwise</returns>
        public bool TryResolve(Type resolveType, ResolveOptions options, out object resolvedType) {
            try {
                resolvedType = this.Resolve(resolveType, options);
                return true;
            }
            catch (TinyIoCResolutionException) {
                resolvedType = null;
                return false;
            }
        }

        /// <summary>
        ///     Attemps to resolve a type using the default options and given name
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="name">Name of registration</param>
        /// <param name="resolvedType">Resolved type or default if resolve fails</param>
        /// <returns>True if resolved sucessfully, false otherwise</returns>
        public bool TryResolve(Type resolveType, string name, out object resolvedType) {
            try {
                resolvedType = this.Resolve(resolveType, name);
                return true;
            }
            catch (TinyIoCResolutionException) {
                resolvedType = null;
                return false;
            }
        }

        /// <summary>
        ///     Attemps to resolve a type using the given options and name
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="name">Name of registration</param>
        /// <param name="options">Resolution options</param>
        /// <param name="resolvedType">Resolved type or default if resolve fails</param>
        /// <returns>True if resolved sucessfully, false otherwise</returns>
        public bool TryResolve(Type resolveType, string name, ResolveOptions options, out object resolvedType) {
            try {
                resolvedType = this.Resolve(resolveType, name, options);
                return true;
            }
            catch (TinyIoCResolutionException) {
                resolvedType = null;
                return false;
            }
        }

        /// <summary>
        ///     Attemps to resolve a type using the default options and supplied constructor parameters
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="parameters">User specified constructor parameters</param>
        /// <param name="resolvedType">Resolved type or default if resolve fails</param>
        /// <returns>True if resolved sucessfully, false otherwise</returns>
        public bool TryResolve(Type resolveType, NamedParameterOverloads parameters, out object resolvedType) {
            try {
                resolvedType = this.Resolve(resolveType, parameters);
                return true;
            }
            catch (TinyIoCResolutionException) {
                resolvedType = null;
                return false;
            }
        }

        /// <summary>
        ///     Attemps to resolve a type using the default options and supplied name and constructor parameters
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="name">Name of registration</param>
        /// <param name="parameters">User specified constructor parameters</param>
        /// <param name="resolvedType">Resolved type or default if resolve fails</param>
        /// <returns>True if resolved sucessfully, false otherwise</returns>
        public bool TryResolve(Type resolveType, string name, NamedParameterOverloads parameters,
            out object resolvedType) {
            try {
                resolvedType = this.Resolve(resolveType, name, parameters);
                return true;
            }
            catch (TinyIoCResolutionException) {
                resolvedType = null;
                return false;
            }
        }

        /// <summary>
        ///     Attemps to resolve a type using the supplied options and constructor parameters
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="parameters">User specified constructor parameters</param>
        /// <param name="options">Resolution options</param>
        /// <param name="resolvedType">Resolved type or default if resolve fails</param>
        /// <returns>True if resolved sucessfully, false otherwise</returns>
        public bool TryResolve(Type resolveType, NamedParameterOverloads parameters, ResolveOptions options,
            out object resolvedType) {
            try {
                resolvedType = this.Resolve(resolveType, parameters, options);
                return true;
            }
            catch (TinyIoCResolutionException) {
                resolvedType = null;
                return false;
            }
        }

        /// <summary>
        ///     Attemps to resolve a type using the supplied name, options and constructor parameters
        /// </summary>
        /// <param name="resolveType">Type to resolve</param>
        /// <param name="name">Name of registration</param>
        /// <param name="parameters">User specified constructor parameters</param>
        /// <param name="options">Resolution options</param>
        /// <param name="resolvedType">Resolved type or default if resolve fails</param>
        /// <returns>True if resolved sucessfully, false otherwise</returns>
        public bool TryResolve(Type resolveType, string name, NamedParameterOverloads parameters, ResolveOptions options,
            out object resolvedType) {
            try {
                resolvedType = this.Resolve(resolveType, name, parameters, options);
                return true;
            }
            catch (TinyIoCResolutionException) {
                resolvedType = null;
                return false;
            }
        }

        /// <summary>
        ///     Attemps to resolve a type using the default options
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="resolvedType">Resolved type or default if resolve fails</param>
        /// <returns>True if resolved sucessfully, false otherwise</returns>
        public bool TryResolve<TResolveType>(out TResolveType resolvedType)
            where TResolveType : class {
            try {
                resolvedType = this.Resolve<TResolveType>();
                return true;
            }
            catch (TinyIoCResolutionException) {
                resolvedType = default(TResolveType);
                return false;
            }
        }

        /// <summary>
        ///     Attemps to resolve a type using the given options
        /// </summary>
        /// <typeparam name="ResolveType">Type to resolve</typeparam>
        /// <param name="options">Resolution options</param>
        /// <param name="resolvedType">Resolved type or default if resolve fails</param>
        /// <returns>True if resolved sucessfully, false otherwise</returns>
        public bool TryResolve<ResolveType>(ResolveOptions options, out ResolveType resolvedType)
            where ResolveType : class {
            try {
                resolvedType = this.Resolve<ResolveType>(options);
                return true;
            }
            catch (TinyIoCResolutionException) {
                resolvedType = default(ResolveType);
                return false;
            }
        }

        /// <summary>
        ///     Attemps to resolve a type using the default options and given name
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="name">Name of registration</param>
        /// <param name="resolvedType">Resolved type or default if resolve fails</param>
        /// <returns>True if resolved sucessfully, false otherwise</returns>
        public bool TryResolve<TResolveType>(string name, out TResolveType resolvedType)
            where TResolveType : class {
            try {
                resolvedType = this.Resolve<TResolveType>(name);
                return true;
            }
            catch (TinyIoCResolutionException) {
                resolvedType = default(TResolveType);
                return false;
            }
        }

        /// <summary>
        ///     Attemps to resolve a type using the given options and name
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="name">Name of registration</param>
        /// <param name="options">Resolution options</param>
        /// <param name="resolvedType">Resolved type or default if resolve fails</param>
        /// <returns>True if resolved sucessfully, false otherwise</returns>
        public bool TryResolve<TResolveType>(string name, ResolveOptions options, out TResolveType resolvedType)
            where TResolveType : class {
            try {
                resolvedType = this.Resolve<TResolveType>(name, options);
                return true;
            }
            catch (TinyIoCResolutionException) {
                resolvedType = default(TResolveType);
                return false;
            }
        }

        /// <summary>
        ///     Attemps to resolve a type using the default options and supplied constructor parameters
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="parameters">User specified constructor parameters</param>
        /// <param name="resolvedType">Resolved type or default if resolve fails</param>
        /// <returns>True if resolved sucessfully, false otherwise</returns>
        public bool TryResolve<TResolveType>(NamedParameterOverloads parameters, out TResolveType resolvedType)
            where TResolveType : class {
            try {
                resolvedType = this.Resolve<TResolveType>(parameters);
                return true;
            }
            catch (TinyIoCResolutionException) {
                resolvedType = default(TResolveType);
                return false;
            }
        }

        /// <summary>
        ///     Attemps to resolve a type using the default options and supplied name and constructor parameters
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="name">Name of registration</param>
        /// <param name="parameters">User specified constructor parameters</param>
        /// <param name="resolvedType">Resolved type or default if resolve fails</param>
        /// <returns>True if resolved sucessfully, false otherwise</returns>
        public bool TryResolve<TResolveType>(string name, NamedParameterOverloads parameters,
            out TResolveType resolvedType)
            where TResolveType : class {
            try {
                resolvedType = this.Resolve<TResolveType>(name, parameters);
                return true;
            }
            catch (TinyIoCResolutionException) {
                resolvedType = default(TResolveType);
                return false;
            }
        }

        /// <summary>
        ///     Attemps to resolve a type using the supplied options and constructor parameters
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="parameters">User specified constructor parameters</param>
        /// <param name="options">Resolution options</param>
        /// <param name="resolvedType">Resolved type or default if resolve fails</param>
        /// <returns>True if resolved sucessfully, false otherwise</returns>
        public bool TryResolve<TResolveType>(NamedParameterOverloads parameters, ResolveOptions options,
            out TResolveType resolvedType)
            where TResolveType : class {
            try {
                resolvedType = this.Resolve<TResolveType>(parameters, options);
                return true;
            }
            catch (TinyIoCResolutionException) {
                resolvedType = default(TResolveType);
                return false;
            }
        }

        /// <summary>
        ///     Attemps to resolve a type using the supplied name, options and constructor parameters
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolve</typeparam>
        /// <param name="name">Name of registration</param>
        /// <param name="parameters">User specified constructor parameters</param>
        /// <param name="options">Resolution options</param>
        /// <param name="resolvedType">Resolved type or default if resolve fails</param>
        /// <returns>True if resolved sucessfully, false otherwise</returns>
        public bool TryResolve<TResolveType>(string name, NamedParameterOverloads parameters, ResolveOptions options,
            out TResolveType resolvedType)
            where TResolveType : class {
            try {
                resolvedType = this.Resolve<TResolveType>(name, parameters, options);
                return true;
            }
            catch (TinyIoCResolutionException) {
                resolvedType = default(TResolveType);
                return false;
            }
        }

        /// <summary>
        ///     Returns all registrations of a type
        /// </summary>
        /// <param name="resolveType">Type to resolveAll</param>
        /// <param name="includeUnnamed">Whether to include un-named (default) registrations</param>
        /// <returns>IEnumerable</returns>
        public IEnumerable<object> ResolveAll(Type resolveType, bool includeUnnamed) {
            return this.ResolveAllInternal(resolveType, includeUnnamed);
        }

        /// <summary>
        ///     Returns all registrations of a type, both named and unnamed
        /// </summary>
        /// <param name="resolveType">Type to resolveAll</param>
        /// <returns>IEnumerable</returns>
        public IEnumerable<object> ResolveAll(Type resolveType) {
            return this.ResolveAll(resolveType, false);
        }

        /// <summary>
        ///     Returns all registrations of a type
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolveAll</typeparam>
        /// <param name="includeUnnamed">Whether to include un-named (default) registrations</param>
        /// <returns>IEnumerable</returns>
        public IEnumerable<TResolveType> ResolveAll<TResolveType>(bool includeUnnamed)
            where TResolveType : class {
            return this.ResolveAll(typeof (TResolveType), includeUnnamed).Cast<TResolveType>();
        }

        /// <summary>
        ///     Returns all registrations of a type, both named and unnamed
        /// </summary>
        /// <typeparam name="TResolveType">Type to resolveAll</typeparam>
        /// <returns>IEnumerable</returns>
        public IEnumerable<TResolveType> ResolveAll<TResolveType>()
            where TResolveType : class {
            return this.ResolveAll<TResolveType>(true);
        }

        /// <summary>
        ///     Attempts to resolve all public property dependencies on the given object.
        /// </summary>
        /// <param name="input">Object to "build up"</param>
        public void BuildUp(object input) {
            this.BuildUpInternal(input, ResolveOptions.Default);
        }

        /// <summary>
        ///     Attempts to resolve all public property dependencies on the given object using the given resolve options.
        /// </summary>
        /// <param name="input">Object to "build up"</param>
        /// <param name="resolveOptions">Resolve options to use</param>
        public void BuildUp(object input, ResolveOptions resolveOptions) {
            this.BuildUpInternal(input, resolveOptions);
        }

        #endregion

        #endregion

        #region Object Factories

        /// <summary>
        ///     Provides custom lifetime management for ASP.Net per-request lifetimes etc.
        /// </summary>
        public interface ITinyIoCObjectLifetimeProvider {
            /// <summary>
            ///     Gets the stored object if it exists, or null if not
            /// </summary>
            /// <returns>Object instance or null</returns>
            object GetObject();

            /// <summary>
            ///     Store the object
            /// </summary>
            /// <param name="value">Object to store</param>
            void SetObject(object value);

            /// <summary>
            ///     Release the object
            /// </summary>
            void ReleaseObject();
        }

        private abstract class ObjectFactoryBase {
            /// <summary>
            ///     Whether to assume this factory sucessfully constructs its objects
            ///     Generally set to true for delegate style factories as CanResolve cannot delve
            ///     into the delegates they contain.
            /// </summary>
            public virtual bool AssumeConstruction {
                get { return false; }
            }

            /// <summary>
            ///     The type the factory instantiates
            /// </summary>
            public abstract Type CreatesType { get; }

            /// <summary>
            ///     Constructor to use, if specified
            /// </summary>
            public ConstructorInfo Constructor { get; private set; }

            public virtual ObjectFactoryBase SingletonVariant {
                get { throw new TinyIoCRegistrationException(this.GetType(), "singleton"); }
            }

            public virtual ObjectFactoryBase MultiInstanceVariant {
                get { throw new TinyIoCRegistrationException(this.GetType(), "multi-instance"); }
            }

            public virtual ObjectFactoryBase StrongReferenceVariant {
                get { throw new TinyIoCRegistrationException(this.GetType(), "strong reference"); }
            }

            public virtual ObjectFactoryBase WeakReferenceVariant {
                get { throw new TinyIoCRegistrationException(this.GetType(), "weak reference"); }
            }

            /// <summary>
            ///     Create the type
            /// </summary>
            /// <param name="requestedType">Type user requested to be resolved</param>
            /// <param name="container">Container that requested the creation</param>
            /// <param name="parameters">Any user parameters passed</param>
            /// <param name="options"></param>
            /// <returns></returns>
            public abstract object GetObject(Type requestedType, TinyIoCContainer container,
                NamedParameterOverloads parameters, ResolveOptions options);

            public virtual ObjectFactoryBase GetCustomObjectLifetimeVariant(
                ITinyIoCObjectLifetimeProvider lifetimeProvider, string errorString) {
                throw new TinyIoCRegistrationException(this.GetType(), errorString);
            }

            public virtual void SetConstructor(ConstructorInfo constructor) {
                this.Constructor = constructor;
            }

            public virtual ObjectFactoryBase GetFactoryForChildContainer(Type type, TinyIoCContainer parent,
                TinyIoCContainer child) {
                return this;
            }
        }

        /// <summary>
        ///     IObjectFactory that creates new instances of types for each resolution
        /// </summary>
        private class MultiInstanceFactory : ObjectFactoryBase {
            private readonly Type _registerImplementation;
            private readonly Type _registerType;

            public MultiInstanceFactory(Type registerType, Type registerImplementation) {
                //#if NETFX_CORE
                //				if (registerImplementation.GetTypeInfo().IsAbstract() || registerImplementation.GetTypeInfo().IsInterface())
                //					throw new TinyIoCRegistrationTypeException(registerImplementation, "MultiInstanceFactory");
                //#else
                if (registerImplementation.IsAbstract() || registerImplementation.IsInterface()) {
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "MultiInstanceFactory");
                }
                //#endif
                if (!IsValidAssignment(registerType, registerImplementation)) {
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "MultiInstanceFactory");
                }

                this._registerType = registerType;
                this._registerImplementation = registerImplementation;
            }

            public override Type CreatesType {
                get { return this._registerImplementation; }
            }

            public override ObjectFactoryBase SingletonVariant {
                get { return new SingletonFactory(this._registerType, this._registerImplementation); }
            }

            public override ObjectFactoryBase MultiInstanceVariant {
                get { return this; }
            }

            public override object GetObject(Type requestedType, TinyIoCContainer container,
                NamedParameterOverloads parameters, ResolveOptions options) {
                try {
                    return container.ConstructType(requestedType, this._registerImplementation, this.Constructor,
                        parameters, options);
                }
                catch (TinyIoCResolutionException ex) {
                    throw new TinyIoCResolutionException(this._registerType, ex);
                }
            }

            public override ObjectFactoryBase GetCustomObjectLifetimeVariant(
                ITinyIoCObjectLifetimeProvider lifetimeProvider, string errorString) {
                return new CustomObjectLifetimeFactory(this._registerType, this._registerImplementation, lifetimeProvider,
                    errorString);
            }
        }

        /// <summary>
        ///     IObjectFactory that invokes a specified delegate to construct the object
        /// </summary>
        private class DelegateFactory : ObjectFactoryBase {
            private readonly Func<TinyIoCContainer, NamedParameterOverloads, object> _factory;
            private readonly Type _registerType;

            public DelegateFactory(Type registerType, Func<TinyIoCContainer, NamedParameterOverloads, object> factory) {
                if (factory == null) {
                    throw new ArgumentNullException("factory");
                }

                this._factory = factory;

                this._registerType = registerType;
            }

            public override bool AssumeConstruction {
                get { return true; }
            }

            public override Type CreatesType {
                get { return this._registerType; }
            }

            public override ObjectFactoryBase WeakReferenceVariant {
                get { return new WeakDelegateFactory(this._registerType, this._factory); }
            }

            public override ObjectFactoryBase StrongReferenceVariant {
                get { return this; }
            }

            public override object GetObject(Type requestedType, TinyIoCContainer container,
                NamedParameterOverloads parameters, ResolveOptions options) {
                try {
                    return this._factory.Invoke(container, parameters);
                }
                catch (Exception ex) {
                    throw new TinyIoCResolutionException(this._registerType, ex);
                }
            }

            public override void SetConstructor(ConstructorInfo constructor) {
                throw new TinyIoCConstructorResolutionException(
                    "Constructor selection is not possible for delegate factory registrations");
            }
        }

        /// <summary>
        ///     IObjectFactory that invokes a specified delegate to construct the object
        ///     Holds the delegate using a weak reference
        /// </summary>
        private class WeakDelegateFactory : ObjectFactoryBase {
            private readonly WeakReference _factory;
            private readonly Type _registerType;

            public WeakDelegateFactory(Type registerType,
                Func<TinyIoCContainer, NamedParameterOverloads, object> factory) {
                if (factory == null) {
                    throw new ArgumentNullException("factory");
                }

                this._factory = new WeakReference(factory);

                this._registerType = registerType;
            }

            public override bool AssumeConstruction {
                get { return true; }
            }

            public override Type CreatesType {
                get { return this._registerType; }
            }

            public override ObjectFactoryBase StrongReferenceVariant {
                get {
                    var factory = this._factory.Target as Func<TinyIoCContainer, NamedParameterOverloads, object>;

                    if (factory == null) {
                        throw new TinyIoCWeakReferenceException(this._registerType);
                    }

                    return new DelegateFactory(this._registerType, factory);
                }
            }

            public override ObjectFactoryBase WeakReferenceVariant {
                get { return this; }
            }

            public override object GetObject(Type requestedType, TinyIoCContainer container,
                NamedParameterOverloads parameters, ResolveOptions options) {
                var factory = this._factory.Target as Func<TinyIoCContainer, NamedParameterOverloads, object>;

                if (factory == null) {
                    throw new TinyIoCWeakReferenceException(this._registerType);
                }

                try {
                    return factory.Invoke(container, parameters);
                }
                catch (Exception ex) {
                    throw new TinyIoCResolutionException(this._registerType, ex);
                }
            }

            public override void SetConstructor(ConstructorInfo constructor) {
                throw new TinyIoCConstructorResolutionException(
                    "Constructor selection is not possible for delegate factory registrations");
            }
        }

        /// <summary>
        ///     Stores an particular instance to return for a type
        /// </summary>
        private class InstanceFactory : ObjectFactoryBase, IDisposable {
            private readonly object _instance;
            private readonly Type _registerImplementation;
            private readonly Type _registerType;

            public InstanceFactory(Type registerType, Type registerImplementation, object instance) {
                if (!IsValidAssignment(registerType, registerImplementation)) {
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "InstanceFactory");
                }

                this._registerType = registerType;
                this._registerImplementation = registerImplementation;
                this._instance = instance;
            }

            public override bool AssumeConstruction {
                get { return true; }
            }

            public override Type CreatesType {
                get { return this._registerImplementation; }
            }

            public override ObjectFactoryBase MultiInstanceVariant {
                get { return new MultiInstanceFactory(this._registerType, this._registerImplementation); }
            }

            public override ObjectFactoryBase WeakReferenceVariant {
                get { return new WeakInstanceFactory(this._registerType, this._registerImplementation, this._instance); }
            }

            public override ObjectFactoryBase StrongReferenceVariant {
                get { return this; }
            }

            public void Dispose() {
                var disposable = this._instance as IDisposable;

                if (disposable != null) {
                    disposable.Dispose();
                }
            }

            public override object GetObject(Type requestedType, TinyIoCContainer container,
                NamedParameterOverloads parameters, ResolveOptions options) {
                return this._instance;
            }

            public override void SetConstructor(ConstructorInfo constructor) {
                throw new TinyIoCConstructorResolutionException(
                    "Constructor selection is not possible for instance factory registrations");
            }
        }

        /// <summary>
        ///     Stores an particular instance to return for a type
        ///     Stores the instance with a weak reference
        /// </summary>
        private class WeakInstanceFactory : ObjectFactoryBase, IDisposable {
            private readonly WeakReference _instance;
            private readonly Type _registerImplementation;
            private readonly Type _registerType;

            public WeakInstanceFactory(Type registerType, Type registerImplementation, object instance) {
                if (!IsValidAssignment(registerType, registerImplementation)) {
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "WeakInstanceFactory");
                }

                this._registerType = registerType;
                this._registerImplementation = registerImplementation;
                this._instance = new WeakReference(instance);
            }

            public override Type CreatesType {
                get { return this._registerImplementation; }
            }

            public override ObjectFactoryBase MultiInstanceVariant {
                get { return new MultiInstanceFactory(this._registerType, this._registerImplementation); }
            }

            public override ObjectFactoryBase WeakReferenceVariant {
                get { return this; }
            }

            public override ObjectFactoryBase StrongReferenceVariant {
                get {
                    var instance = this._instance.Target;

                    if (instance == null) {
                        throw new TinyIoCWeakReferenceException(this._registerType);
                    }

                    return new InstanceFactory(this._registerType, this._registerImplementation, instance);
                }
            }

            public void Dispose() {
                var disposable = this._instance.Target as IDisposable;

                if (disposable != null) {
                    disposable.Dispose();
                }
            }

            public override object GetObject(Type requestedType, TinyIoCContainer container,
                NamedParameterOverloads parameters, ResolveOptions options) {
                var instance = this._instance.Target;

                if (instance == null) {
                    throw new TinyIoCWeakReferenceException(this._registerType);
                }

                return instance;
            }

            public override void SetConstructor(ConstructorInfo constructor) {
                throw new TinyIoCConstructorResolutionException(
                    "Constructor selection is not possible for instance factory registrations");
            }
        }

        /// <summary>
        ///     A factory that lazy instantiates a type and always returns the same instance
        /// </summary>
        private class SingletonFactory : ObjectFactoryBase, IDisposable {
            private readonly Type _registerImplementation;
            private readonly Type _registerType;
            private readonly object _singletonLock = new object();
            private object _current;

            public SingletonFactory(Type registerType, Type registerImplementation) {
                //#if NETFX_CORE
                //				if (registerImplementation.GetTypeInfo().IsAbstract() || registerImplementation.GetTypeInfo().IsInterface())
                //#else
                if (registerImplementation.IsAbstract() || registerImplementation.IsInterface()) {
                    //#endif
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "SingletonFactory");
                }

                if (!IsValidAssignment(registerType, registerImplementation)) {
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "SingletonFactory");
                }

                this._registerType = registerType;
                this._registerImplementation = registerImplementation;
            }

            public override Type CreatesType {
                get { return this._registerImplementation; }
            }

            public override ObjectFactoryBase SingletonVariant {
                get { return this; }
            }

            public override ObjectFactoryBase MultiInstanceVariant {
                get { return new MultiInstanceFactory(this._registerType, this._registerImplementation); }
            }

            public void Dispose() {
                var disposable = this._current as IDisposable;

                if (disposable != null) {
                    disposable.Dispose();
                }
            }

            public override object GetObject(Type requestedType, TinyIoCContainer container,
                NamedParameterOverloads parameters, ResolveOptions options) {
                if (parameters.Count != 0) {
                    throw new ArgumentException("Cannot specify parameters for singleton types");
                }

                lock (this._singletonLock)
                    if (this._current == null) {
                        this._current = container.ConstructType(requestedType, this._registerImplementation,
                            this.Constructor, options);
                    }

                return this._current;
            }

            public override ObjectFactoryBase GetCustomObjectLifetimeVariant(
                ITinyIoCObjectLifetimeProvider lifetimeProvider, string errorString) {
                return new CustomObjectLifetimeFactory(this._registerType, this._registerImplementation, lifetimeProvider,
                    errorString);
            }

            public override ObjectFactoryBase GetFactoryForChildContainer(Type type, TinyIoCContainer parent,
                TinyIoCContainer child) {
                // We make sure that the singleton is constructed before the child container takes the factory.
                // Otherwise the results would vary depending on whether or not the parent container had resolved
                // the type before the child container does.
                this.GetObject(type, parent, NamedParameterOverloads.Default, ResolveOptions.Default);
                return this;
            }
        }

        /// <summary>
        ///     A factory that offloads lifetime to an external lifetime provider
        /// </summary>
        private class CustomObjectLifetimeFactory : ObjectFactoryBase, IDisposable {
            private readonly ITinyIoCObjectLifetimeProvider _lifetimeProvider;
            private readonly Type _registerImplementation;
            private readonly Type _registerType;
            private readonly object _singletonLock = new object();

            public CustomObjectLifetimeFactory(Type registerType, Type registerImplementation,
                ITinyIoCObjectLifetimeProvider lifetimeProvider, string errorMessage) {
                if (lifetimeProvider == null) {
                    throw new ArgumentNullException("lifetimeProvider", "lifetimeProvider is null.");
                }

                if (!IsValidAssignment(registerType, registerImplementation)) {
                    throw new TinyIoCRegistrationTypeException(registerImplementation, "SingletonFactory");
                }

                //#if NETFX_CORE
                //				if (registerImplementation.GetTypeInfo().IsAbstract() || registerImplementation.GetTypeInfo().IsInterface())
                //#else
                if (registerImplementation.IsAbstract() || registerImplementation.IsInterface()) {
                    //#endif
                    throw new TinyIoCRegistrationTypeException(registerImplementation, errorMessage);
                }

                this._registerType = registerType;
                this._registerImplementation = registerImplementation;
                this._lifetimeProvider = lifetimeProvider;
            }

            public override Type CreatesType {
                get { return this._registerImplementation; }
            }

            public override ObjectFactoryBase SingletonVariant {
                get {
                    this._lifetimeProvider.ReleaseObject();
                    return new SingletonFactory(this._registerType, this._registerImplementation);
                }
            }

            public override ObjectFactoryBase MultiInstanceVariant {
                get {
                    this._lifetimeProvider.ReleaseObject();
                    return new MultiInstanceFactory(this._registerType, this._registerImplementation);
                }
            }

            public void Dispose() {
                this._lifetimeProvider.ReleaseObject();
            }

            public override object GetObject(Type requestedType, TinyIoCContainer container,
                NamedParameterOverloads parameters, ResolveOptions options) {
                object current;

                lock (this._singletonLock) {
                    current = this._lifetimeProvider.GetObject();
                    if (current != null) {
                        return current;
                    }
                    current = container.ConstructType(requestedType, this._registerImplementation, this.Constructor,
                        options);
                    this._lifetimeProvider.SetObject(current);
                }

                return current;
            }

            public override ObjectFactoryBase GetCustomObjectLifetimeVariant(
                ITinyIoCObjectLifetimeProvider lifetimeProvider, string errorString) {
                this._lifetimeProvider.ReleaseObject();
                return new CustomObjectLifetimeFactory(this._registerType, this._registerImplementation, lifetimeProvider,
                    errorString);
            }

            public override ObjectFactoryBase GetFactoryForChildContainer(Type type, TinyIoCContainer parent,
                TinyIoCContainer child) {
                // We make sure that the singleton is constructed before the child container takes the factory.
                // Otherwise the results would vary depending on whether or not the parent container had resolved
                // the type before the child container does.
                this.GetObject(type, parent, NamedParameterOverloads.Default, ResolveOptions.Default);
                return this;
            }
        }

        #endregion

        #region Singleton Container

        private static readonly TinyIoCContainer _Current = new TinyIoCContainer();

        static TinyIoCContainer() {}

        /// <summary>
        ///     Lazy created Singleton instance of the container for simple scenarios
        /// </summary>
        public static TinyIoCContainer Current {
            get { return _Current; }
        }

        #endregion

        #region Type Registrations

        public sealed class TypeRegistration {
            private readonly int _hashCode;

            public TypeRegistration(Type type)
                : this(type, string.Empty) {}

            public TypeRegistration(Type type, string name) {
                this.Type = type;
                this.Name = name;

                this._hashCode = string.Concat(this.Type.FullName, "|", this.Name).GetHashCode();
            }

            public Type Type { get; private set; }
            public string Name { get; private set; }

            public override bool Equals(object obj) {
                var typeRegistration = obj as TypeRegistration;

                if (typeRegistration == null) {
                    return false;
                }

                if (this.Type != typeRegistration.Type) {
                    return false;
                }

                if (string.Compare(this.Name, typeRegistration.Name, StringComparison.Ordinal) != 0) {
                    return false;
                }

                return true;
            }

            public override int GetHashCode() {
                return this._hashCode;
            }
        }

        private readonly SafeDictionary<TypeRegistration, ObjectFactoryBase> _registeredTypes;
#if USE_OBJECT_CONSTRUCTOR
        private delegate object ObjectConstructor(params object[] parameters);

        private static readonly SafeDictionary<ConstructorInfo, ObjectConstructor> ObjectConstructorCache =
            new SafeDictionary<ConstructorInfo, ObjectConstructor>();
#endif

        #endregion

        #region Constructors

        public TinyIoCContainer() {
            this._registeredTypes = new SafeDictionary<TypeRegistration, ObjectFactoryBase>();

            this.RegisterDefaultTypes();
        }

        private readonly TinyIoCContainer _parent;

        private TinyIoCContainer(TinyIoCContainer parent)
            : this() {
            this._parent = parent;
        }

        #endregion

        #region Internal Methods

        private readonly object _autoRegisterLock = new object();

        private void AutoRegisterInternal(IEnumerable<Assembly> assemblies,
            DuplicateImplementationActions duplicateAction, Func<Type, bool> registrationPredicate) {
            lock (this._autoRegisterLock) {
                var types =
                    assemblies.SelectMany(a => a.SafeGetTypes())
                        .Where(t => !IsIgnoredType(t, registrationPredicate))
                        .ToList();

                var concreteTypes = from type in types
                    where
                        type.IsClass() && (type.IsAbstract() == false) &&
                        (type != this.GetType() && (type.DeclaringType != this.GetType()) &&
                         (!type.IsGenericTypeDefinition()))
                    select type;

                var enumerable = concreteTypes as Type[] ?? concreteTypes.ToArray();
                foreach (var type in enumerable) {
                    try {
                        this.RegisterInternal(type, string.Empty, GetDefaultObjectFactory(type, type));
                    }
                    catch (MethodAccessException) {
                        // Ignore methods we can't access - added for Silverlight
                    }
                }

                var abstractInterfaceTypes = from type in types
                    where
                        ((type.IsInterface() || type.IsAbstract()) && (type.DeclaringType != this.GetType()) &&
                         (!type.IsGenericTypeDefinition()))
                    select type;

                foreach (var type in abstractInterfaceTypes) {
                    var localType = type;
                    var implementations = from implementationType in enumerable
                        where localType.IsAssignableFrom(implementationType)
                        select implementationType;

                    var implementationTypes = implementations as Type[] ?? implementations.ToArray();
                    if (implementationTypes.Count() > 1) {
                        switch (duplicateAction) {
                            case DuplicateImplementationActions.Fail:
                                throw new TinyIoCAutoRegistrationException(type, implementationTypes);
                            case DuplicateImplementationActions.RegisterMultiple:
                                this.RegisterMultiple(type, implementationTypes);
                                break;
                        }
                    }

                    var firstImplementation = implementationTypes.FirstOrDefault();
                    if (firstImplementation == null) {
                        continue;
                    }
                    try {
                        this.RegisterInternal(type, string.Empty,
                            GetDefaultObjectFactory(type, firstImplementation));
                    }
                    catch (MethodAccessException) {
                        // Ignore methods we can't access - added for Silverlight
                    }
                }
            }
        }

        private static bool IsIgnoredAssembly(Assembly assembly) {
            // TODO - find a better way to remove "system" assemblies from the auto registration
            var ignoreChecks = new List<Func<Assembly, bool>> {
                asm => asm.FullName.StartsWith("Microsoft.", StringComparison.Ordinal),
                asm => asm.FullName.StartsWith("System.", StringComparison.Ordinal),
                asm => asm.FullName.StartsWith("System,", StringComparison.Ordinal),
                asm => asm.FullName.StartsWith("CR_ExtUnitTest", StringComparison.Ordinal),
                asm => asm.FullName.StartsWith("mscorlib,", StringComparison.Ordinal),
                asm => asm.FullName.StartsWith("CR_VSTest", StringComparison.Ordinal),
                asm => asm.FullName.StartsWith("DevExpress.CodeRush", StringComparison.Ordinal)
            };

            return ignoreChecks.Any(check => check(assembly));
        }

        private static bool IsIgnoredType(Type type, Func<Type, bool> registrationPredicate) {
            // TODO - find a better way to remove "system" types from the auto registration
            var ignoreChecks = new List<Func<Type, bool>> {
                t => t.FullName.StartsWith("System.", StringComparison.Ordinal),
                t => t.FullName.StartsWith("Microsoft.", StringComparison.Ordinal),
                t => t.IsPrimitive(),
#if !UNBOUND_GENERICS_GETCONSTRUCTORS
                t => t.IsGenericTypeDefinition(),
#endif
                t =>
                    (t.GetConstructors(BindingFlags.Instance | BindingFlags.Public).Length == 0) &&
                    !(t.IsInterface() || t.IsAbstract())
            };

            if (registrationPredicate != null) {
                ignoreChecks.Add(t => !registrationPredicate(t));
            }

            return ignoreChecks.Any(check => check(type));
        }

        private void RegisterDefaultTypes() {
            this.Register(this);

#if TINYMESSENGER
    // Only register the TinyMessenger singleton if we are the root container
            if (_Parent == null)
                Register<TinyMessenger.ITinyMessengerHub, TinyMessenger.TinyMessengerHub>();
#endif
        }

        private ObjectFactoryBase GetCurrentFactory(TypeRegistration registration) {
            ObjectFactoryBase current;

            this._registeredTypes.TryGetValue(registration, out current);

            return current;
        }

        private RegisterOptions RegisterInternal(Type registerType, string name, ObjectFactoryBase factory) {
            var typeRegistration = new TypeRegistration(registerType, name);

            return this.AddUpdateRegistration(typeRegistration, factory);
        }

        private RegisterOptions AddUpdateRegistration(TypeRegistration typeRegistration, ObjectFactoryBase factory) {
            this._registeredTypes[typeRegistration] = factory;

            return new RegisterOptions(this, typeRegistration);
        }

        private void RemoveRegistration(TypeRegistration typeRegistration) {
            this._registeredTypes.Remove(typeRegistration);
        }

        private static ObjectFactoryBase GetDefaultObjectFactory(Type registerType, Type registerImplementation) {
            //#if NETFX_CORE
            //			if (registerType.GetTypeInfo().IsInterface() || registerType.GetTypeInfo().IsAbstract())
            //#else
            if (registerType.IsInterface() || registerType.IsAbstract()) {
                //#endif
                return new SingletonFactory(registerType, registerImplementation);
            }

            return new MultiInstanceFactory(registerType, registerImplementation);
        }

        private bool CanResolveInternal(TypeRegistration registration, NamedParameterOverloads parameters,
            ResolveOptions options) {
            if (parameters == null) {
                throw new ArgumentNullException("parameters");
            }

            var checkType = registration.Type;
            var name = registration.Name;

            ObjectFactoryBase factory;
            if (this._registeredTypes.TryGetValue(new TypeRegistration(checkType, name), out factory)) {
                if (factory.AssumeConstruction) {
                    return true;
                }

                if (factory.Constructor == null) {
                    return (this.GetBestConstructor(factory.CreatesType, parameters, options) != null) ? true : false;
                }
                return this.CanConstruct(factory.Constructor, parameters, options);
            }

#if RESOLVE_OPEN_GENERICS
            if (checkType.IsInterface && checkType.IsGenericType) {
                // if the type is registered as an open generic, then see if the open generic is registered
                if (this._registeredTypes.TryGetValue(new TypeRegistration(checkType.GetGenericTypeDefinition(), name),
                    out factory)) {
                    if (factory.AssumeConstruction) {
                        return true;
                    }

                    if (factory.Constructor == null) {
                        return (this.GetBestConstructor(factory.CreatesType, parameters, options) != null)
                            ? true
                            : false;
                    }
                    return this.CanConstruct(factory.Constructor, parameters, options);
                }
            }
#endif

            // Fail if requesting named resolution and settings set to fail if unresolved
            // Or bubble up if we have a parent
            if (!string.IsNullOrEmpty(name) &&
                options.NamedResolutionFailureAction == NamedResolutionFailureActions.Fail) {
                return (this._parent != null) && this._parent.CanResolveInternal(registration, parameters, options);
            }

            // Attemped unnamed fallback container resolution if relevant and requested
            if (!string.IsNullOrEmpty(name) &&
                options.NamedResolutionFailureAction == NamedResolutionFailureActions.AttemptUnnamedResolution) {
                if (this._registeredTypes.TryGetValue(new TypeRegistration(checkType), out factory)) {
                    if (factory.AssumeConstruction) {
                        return true;
                    }

                    return (this.GetBestConstructor(factory.CreatesType, parameters, options) != null);
                }
            }

            // Check if type is an automatic lazy factory request
            if (IsAutomaticLazyFactoryRequest(checkType)) {
                return true;
            }

            // Check if type is an IEnumerable<ResolveType>
            if (IsIEnumerableRequest(registration.Type)) {
                return true;
            }

            // Attempt unregistered construction if possible and requested
            // If we cant', bubble if we have a parent
            if ((options.UnregisteredResolutionAction == UnregisteredResolutionActions.AttemptResolve) ||
                (checkType.IsGenericType() &&
                 options.UnregisteredResolutionAction == UnregisteredResolutionActions.GenericsOnly)) {
                return (this.GetBestConstructor(checkType, parameters, options) != null) || (this._parent != null) && this._parent.CanResolveInternal(registration, parameters, options);
            }

            // Bubble resolution up the container tree if we have a parent
            return this._parent != null && this._parent.CanResolveInternal(registration, parameters, options);
        }

        private static bool IsIEnumerableRequest(Type type) {
            if (!type.IsGenericType()) {
                return false;
            }

            var genericType = type.GetGenericTypeDefinition();

            return genericType == typeof (IEnumerable<>);
        }

        private static bool IsAutomaticLazyFactoryRequest(Type type) {
            if (!type.IsGenericType()) {
                return false;
            }

            var genericType = type.GetGenericTypeDefinition();

            // Just a func
            if (genericType == typeof (Func<>)) {
                return true;
            }

            // 2 parameter func with string as first parameter (name)
            //#if NETFX_CORE
            //			if ((genericType == typeof(Func<,>) && type.GetTypeInfo().GenericTypeArguments[0] == typeof(string)))
            //#else
            if ((genericType == typeof (Func<,>) && type.GetGenericArguments()[0] == typeof (string))) {
                //#endif
                return true;
            }

            // 3 parameter func with string as first parameter (name) and IDictionary<string, object> as second (parameters)
            //#if NETFX_CORE
            //			if ((genericType == typeof(Func<,,>) && type.GetTypeInfo().GenericTypeArguments[0] == typeof(string) && type.GetTypeInfo().GenericTypeArguments[1] == typeof(IDictionary<String, object>)))
            //#else
            if ((genericType == typeof (Func<,,>) && type.GetGenericArguments()[0] == typeof (string) &&
                 type.GetGenericArguments()[1] == typeof (IDictionary<string, object>))) {
                //#endif
                return true;
            }

            return false;
        }

        private ObjectFactoryBase GetParentObjectFactory(TypeRegistration registration) {
            if (this._parent == null) {
                return null;
            }

            ObjectFactoryBase factory;
            return this._parent._registeredTypes.TryGetValue(registration, out factory) ? factory.GetFactoryForChildContainer(registration.Type, this._parent, this) : this._parent.GetParentObjectFactory(registration);
        }

        private object ResolveInternal(TypeRegistration registration, NamedParameterOverloads parameters,
            ResolveOptions options) {
            ObjectFactoryBase factory;

            // Attempt container resolution
            if (this._registeredTypes.TryGetValue(registration, out factory)) {
                try {
                    return factory.GetObject(registration.Type, this, parameters, options);
                }
                catch (TinyIoCResolutionException) {
                    throw;
                }
                catch (Exception ex) {
                    throw new TinyIoCResolutionException(registration.Type, ex);
                }
            }

#if RESOLVE_OPEN_GENERICS
            // Attempt container resolution of open generic
            if (registration.Type.IsGenericType()) {
                var openTypeRegistration = new TypeRegistration(registration.Type.GetGenericTypeDefinition(),
                    registration.Name);

                if (this._registeredTypes.TryGetValue(openTypeRegistration, out factory)) {
                    try {
                        return factory.GetObject(registration.Type, this, parameters, options);
                    }
                    catch (TinyIoCResolutionException) {
                        throw;
                    }
                    catch (Exception ex) {
                        throw new TinyIoCResolutionException(registration.Type, ex);
                    }
                }
            }
#endif

            // Attempt to get a factory from parent if we can
            var bubbledObjectFactory = this.GetParentObjectFactory(registration);
            if (bubbledObjectFactory != null) {
                try {
                    return bubbledObjectFactory.GetObject(registration.Type, this, parameters, options);
                }
                catch (TinyIoCResolutionException) {
                    throw;
                }
                catch (Exception ex) {
                    throw new TinyIoCResolutionException(registration.Type, ex);
                }
            }

            // Fail if requesting named resolution and settings set to fail if unresolved
            if (!string.IsNullOrEmpty(registration.Name) &&
                options.NamedResolutionFailureAction == NamedResolutionFailureActions.Fail) {
                throw new TinyIoCResolutionException(registration.Type);
            }

            // Attemped unnamed fallback container resolution if relevant and requested
            if (!string.IsNullOrEmpty(registration.Name) &&
                options.NamedResolutionFailureAction == NamedResolutionFailureActions.AttemptUnnamedResolution) {
                if (this._registeredTypes.TryGetValue(new TypeRegistration(registration.Type, string.Empty), out factory)) {
                    try {
                        return factory.GetObject(registration.Type, this, parameters, options);
                    }
                    catch (TinyIoCResolutionException) {
                        throw;
                    }
                    catch (Exception ex) {
                        throw new TinyIoCResolutionException(registration.Type, ex);
                    }
                }
            }

#if EXPRESSIONS
            // Attempt to construct an automatic lazy factory if possible
            if (IsAutomaticLazyFactoryRequest(registration.Type)) {
                return this.GetLazyAutomaticFactoryRequest(registration.Type);
            }
#endif
            if (IsIEnumerableRequest(registration.Type)) {
                return this.GetIEnumerableRequest(registration.Type);
            }

            // Attempt unregistered construction if possible and requested
            if ((options.UnregisteredResolutionAction != UnregisteredResolutionActions.AttemptResolve) &&
                (!registration.Type.IsGenericType() ||
                 options.UnregisteredResolutionAction != UnregisteredResolutionActions.GenericsOnly)) {
                throw new TinyIoCResolutionException(registration.Type);
            }
            if (!registration.Type.IsAbstract() && !registration.Type.IsInterface()) {
                return this.ConstructType(null, registration.Type, parameters, options);
            }

            // Unable to resolve - throw
            throw new TinyIoCResolutionException(registration.Type);
        }

#if EXPRESSIONS
        private object GetLazyAutomaticFactoryRequest(Type type) {
            if (!type.IsGenericType()) {
                return null;
            }

            var genericType = type.GetGenericTypeDefinition();
            //#if NETFX_CORE
            //			Type[] genericArguments = type.GetTypeInfo().GenericTypeArguments.ToArray();
            //#else
            var genericArguments = type.GetGenericArguments();
            //#endif

            // Just a func
            if (genericType == typeof (Func<>)) {
                var returnType = genericArguments[0];

                //#if NETFX_CORE
                //				MethodInfo resolveMethod = typeof(TinyIoCContainer).GetTypeInfo().GetDeclaredMethods("Resolve").First(mi => !mi.GetParameters().Any());
                //#else
                var resolveMethod = typeof (TinyIoCContainer).GetMethod("Resolve", new Type[] {});
                //#endif
                resolveMethod = resolveMethod.MakeGenericMethod(returnType);

                var resolveCall = Expression.Call(Expression.Constant(this), resolveMethod);

                var resolveLambda = Expression.Lambda(resolveCall).Compile();

                return resolveLambda;
            }

            // 2 parameter func with string as first parameter (name)
            if ((genericType == typeof (Func<,>)) && (genericArguments[0] == typeof (string))) {
                var returnType = genericArguments[1];

                //#if NETFX_CORE
                //				MethodInfo resolveMethod = typeof(TinyIoCContainer).GetTypeInfo().GetDeclaredMethods("Resolve").First(mi => mi.GetParameters().Length == 1 && mi.GetParameters()[0].GetType() == typeof(String));
                //#else
                var resolveMethod = typeof (TinyIoCContainer).GetMethod("Resolve", new[] {typeof (string)});
                //#endif
                resolveMethod = resolveMethod.MakeGenericMethod(returnType);

                ParameterExpression[] resolveParameters = {Expression.Parameter(typeof (string), "name")};
                var resolveCall = Expression.Call(Expression.Constant(this), resolveMethod, resolveParameters);

                var resolveLambda = Expression.Lambda(resolveCall, resolveParameters).Compile();

                return resolveLambda;
            }

            // 3 parameter func with string as first parameter (name) and IDictionary<string, object> as second (parameters)
            //#if NETFX_CORE
            //			if ((genericType == typeof(Func<,,>) && type.GenericTypeArguments[0] == typeof(string) && type.GenericTypeArguments[1] == typeof(IDictionary<string, object>)))
            //#else
            if ((genericType == typeof (Func<,,>) && type.GetGenericArguments()[0] == typeof (string) &&
                 type.GetGenericArguments()[1] == typeof (IDictionary<string, object>)))
                //#endif
            {
                var returnType = genericArguments[2];

                var name = Expression.Parameter(typeof (string), "name");
                var parameters = Expression.Parameter(typeof (IDictionary<string, object>), "parameters");

                //#if NETFX_CORE
                //				MethodInfo resolveMethod = typeof(TinyIoCContainer).GetTypeInfo().GetDeclaredMethods("Resolve").First(mi => mi.GetParameters().Length == 2 && mi.GetParameters()[0].GetType() == typeof(String) && mi.GetParameters()[1].GetType() == typeof(NamedParameterOverloads));
                //#else
                var resolveMethod = typeof (TinyIoCContainer).GetMethod("Resolve",
                    new[] {typeof (string), typeof (NamedParameterOverloads)});
                //#endif
                resolveMethod = resolveMethod.MakeGenericMethod(returnType);

                var resolveCall = Expression.Call(Expression.Constant(this), resolveMethod, name,
                    Expression.Call(typeof (NamedParameterOverloads), "FromIDictionary", null, parameters));

                var resolveLambda = Expression.Lambda(resolveCall, name, parameters).Compile();

                return resolveLambda;
            }

            throw new TinyIoCResolutionException(type);
        }
#endif

        private object GetIEnumerableRequest(Type type) {
            //#if NETFX_CORE
            //			var genericResolveAllMethod = this.GetType().GetGenericMethod("ResolveAll", type.GenericTypeArguments, new[] { typeof(bool) });
            //#else
            var genericResolveAllMethod = this.GetType()
                .GetGenericMethod(BindingFlags.Public | BindingFlags.Instance, "ResolveAll", type.GetGenericArguments(),
                    new[] {typeof (bool)});
            //#endif

            return genericResolveAllMethod.Invoke(this, new object[] {false});
        }

        private bool CanConstruct(ConstructorInfo ctor, NamedParameterOverloads parameters, ResolveOptions options) {
            if (parameters == null) {
                throw new ArgumentNullException("parameters");
            }

            foreach (var parameter in ctor.GetParameters()) {
                if (string.IsNullOrEmpty(parameter.Name)) {
                    return false;
                }

                var isParameterOverload = parameters.ContainsKey(parameter.Name);

                //#if NETFX_CORE                
                //				if (parameter.ParameterType.GetTypeInfo().IsPrimitive && !isParameterOverload)
                //#else
                if (parameter.ParameterType.IsPrimitive() && !isParameterOverload) {
                    //#endif
                    return false;
                }

                if (!isParameterOverload &&
                    !this.CanResolveInternal(new TypeRegistration(parameter.ParameterType),
                        NamedParameterOverloads.Default, options)) {
                    return false;
                }
            }

            return true;
        }

        private ConstructorInfo GetBestConstructor(Type type, NamedParameterOverloads parameters, ResolveOptions options) {
            if (parameters == null) {
                throw new ArgumentNullException("parameters");
            }

            //#if NETFX_CORE
            //			if (type.GetTypeInfo().IsValueType)
            //#else
            if (type.IsValueType()) {
                //#endif
                return null;
            }

            // Get constructors in reverse order based on the number of parameters
            // i.e. be as "greedy" as possible so we satify the most amount of dependencies possible
            var ctors = GetTypeConstructors(type);

            return ctors.FirstOrDefault(ctor => this.CanConstruct(ctor, parameters, options));
        }

        private static IEnumerable<ConstructorInfo> GetTypeConstructors(Type type) {
            //#if NETFX_CORE
            //			return type.GetTypeInfo().DeclaredConstructors.OrderByDescending(ctor => ctor.GetParameters().Count());
            //#else
            return type.GetConstructors().OrderByDescending(ctor => ctor.GetParameters().Count());
            //#endif
        }

        private object ConstructType(Type requestedType, Type implementationType, ResolveOptions options) {
            return this.ConstructType(requestedType, implementationType, null, NamedParameterOverloads.Default, options);
        }

        private object ConstructType(Type requestedType, Type implementationType, ConstructorInfo constructor,
            ResolveOptions options) {
            return this.ConstructType(requestedType, implementationType, constructor, NamedParameterOverloads.Default,
                options);
        }

        private object ConstructType(Type requestedType, Type implementationType, NamedParameterOverloads parameters,
            ResolveOptions options) {
            return this.ConstructType(requestedType, implementationType, null, parameters, options);
        }

        private object ConstructType(Type requestedType, Type implementationType, ConstructorInfo constructor,
            NamedParameterOverloads parameters, ResolveOptions options) {
            var typeToConstruct = implementationType;

#if RESOLVE_OPEN_GENERICS
            if (implementationType.IsGenericTypeDefinition()) {
                if (requestedType == null || !requestedType.IsGenericType() ||
                    !requestedType.GetGenericArguments().Any()) {
                    throw new TinyIoCResolutionException(typeToConstruct);
                }

                typeToConstruct = typeToConstruct.MakeGenericType(requestedType.GetGenericArguments());
            }
#endif
            if (constructor == null) {
                // Try and get the best constructor that we can construct
                // if we can't construct any then get the constructor
                // with the least number of parameters so we can throw a meaningful
                // resolve exception
                constructor = this.GetBestConstructor(typeToConstruct, parameters, options) ??
                              GetTypeConstructors(typeToConstruct).LastOrDefault();
            }

            if (constructor == null) {
                throw new TinyIoCResolutionException(typeToConstruct);
            }

            var ctorParams = constructor.GetParameters();
            var args = new object[ctorParams.Count()];

            for (var parameterIndex = 0; parameterIndex < ctorParams.Count(); parameterIndex++) {
                var currentParam = ctorParams[parameterIndex];

                try {
                    args[parameterIndex] = parameters.ContainsKey(currentParam.Name)
                        ? parameters[currentParam.Name]
                        : this.ResolveInternal(
                            new TypeRegistration(currentParam.ParameterType),
                            NamedParameterOverloads.Default,
                            options);
                }
                catch (TinyIoCResolutionException ex) {
                    // If a constructor parameter can't be resolved
                    // it will throw, so wrap it and throw that this can't
                    // be resolved.
                    throw new TinyIoCResolutionException(typeToConstruct, ex);
                }
                catch (Exception ex) {
                    throw new TinyIoCResolutionException(typeToConstruct, ex);
                }
            }

            try {
#if USE_OBJECT_CONSTRUCTOR
                var constructionDelegate = CreateObjectConstructionDelegateWithCache(constructor);
                return constructionDelegate.Invoke(args);
#else
                return constructor.Invoke(args);
#endif
            }
            catch (Exception ex) {
                throw new TinyIoCResolutionException(typeToConstruct, ex);
            }
        }

#if USE_OBJECT_CONSTRUCTOR
        private static ObjectConstructor CreateObjectConstructionDelegateWithCache(ConstructorInfo constructor) {
            ObjectConstructor objectConstructor;
            if (ObjectConstructorCache.TryGetValue(constructor, out objectConstructor)) {
                return objectConstructor;
            }

            // We could lock the cache here, but there's no real side
            // effect to two threads creating the same ObjectConstructor
            // at the same time, compared to the cost of a lock for 
            // every creation.
            var constructorParams = constructor.GetParameters();
            var lambdaParams = Expression.Parameter(typeof (object[]), "parameters");
            var newParams = new Expression[constructorParams.Length];

            for (var i = 0; i < constructorParams.Length; i++) {
                var paramsParameter = Expression.ArrayIndex(lambdaParams, Expression.Constant(i));

                newParams[i] = Expression.Convert(paramsParameter, constructorParams[i].ParameterType);
            }

            var newExpression = Expression.New(constructor, newParams);

            var constructionLambda = Expression.Lambda(typeof (ObjectConstructor), newExpression, lambdaParams);

            objectConstructor = (ObjectConstructor) constructionLambda.Compile();

            ObjectConstructorCache[constructor] = objectConstructor;
            return objectConstructor;
        }
#endif

        private void BuildUpInternal(object input, ResolveOptions resolveOptions) {
            //#if NETFX_CORE
            //			var properties = from property in input.GetType().GetTypeInfo().DeclaredProperties
            //							 where (property.GetMethod != null) && (property.SetMethod != null) && !property.PropertyType.GetTypeInfo().IsValueType
            //							 select property;
            //#else
            var properties = from property in input.GetType().GetProperties()
                where
                    (property.GetGetMethod() != null) && (property.GetSetMethod() != null) &&
                    !property.PropertyType.IsValueType()
                select property;
            //#endif

            foreach (var property in properties.Where(property => property.GetValue(input, null) == null)) {
                try {
                    property.SetValue(input,
                        this.ResolveInternal(new TypeRegistration(property.PropertyType),
                            NamedParameterOverloads.Default, resolveOptions), null);
                }
                catch (TinyIoCResolutionException) {
                    // Catch any resolution errors and ignore them
                }
            }
        }

        private IEnumerable<TypeRegistration> GetParentRegistrationsForType(Type resolveType) {
            if (this._parent == null) {
                return new TypeRegistration[] {};
            }

            var registrations = this._parent._registeredTypes.Keys.Where(tr => tr.Type == resolveType);

            return registrations.Concat(this._parent.GetParentRegistrationsForType(resolveType));
        }

        private IEnumerable<object> ResolveAllInternal(Type resolveType, bool includeUnnamed) {
            var registrations =
                this._registeredTypes.Keys.Where(tr => tr.Type == resolveType)
                    .Concat(this.GetParentRegistrationsForType(resolveType));

            if (!includeUnnamed) {
                registrations = registrations.Where(tr => tr.Name != string.Empty);
            }

            return
                registrations.Select(
                    registration =>
                        this.ResolveInternal(registration, NamedParameterOverloads.Default, ResolveOptions.Default));
        }

        private static bool IsValidAssignment(Type registerType, Type registerImplementation) {
            //#if NETFX_CORE
            //			var registerTypeDef = registerType.GetTypeInfo();
            //			var registerImplementationDef = registerImplementation.GetTypeInfo();

            //			if (!registerTypeDef.IsGenericTypeDefinition)
            //			{
            //				if (!registerTypeDef.IsAssignableFrom(registerImplementationDef))
            //					return false;
            //			}
            //			else
            //			{
            //				if (registerTypeDef.IsInterface())
            //				{
            //					if (!registerImplementationDef.ImplementedInterfaces.Any(t => t.GetTypeInfo().Name == registerTypeDef.Name))
            //						return false;
            //				}
            //				else if (registerTypeDef.IsAbstract() && registerImplementationDef.BaseType() != registerType)
            //				{
            //					return false;
            //				}
            //			}
            //#else
            if (!registerType.IsGenericTypeDefinition()) {
                if (!registerType.IsAssignableFrom(registerImplementation)) {
                    return false;
                }
            }
            else {
                if (registerType.IsInterface()) {
                    if (!registerImplementation.FindInterfaces((t, o) => t.Name == registerType.Name, null).Any()) {
                        return false;
                    }
                }
                else if (registerType.IsAbstract() && registerImplementation.BaseType() != registerType) {
                    return false;
                }
            }
            //#endif
            return true;
        }

        #endregion

        #region IDisposable Members

        private bool _disposed;

        public void Dispose() {
            if (this._disposed) {
                return;
            }
            this._disposed = true;

            this._registeredTypes.Dispose();

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

// reverse shim for WinRT SR changes...
#if !NETFX_CORE

namespace System.Reflection {
#if TINYIOC_INTERNAL
    internal
#else
    public
#endif
        static class ReverseTypeExtender {
        public static bool IsClass(this Type type) {
            return type.IsClass;
        }

        public static bool IsAbstract(this Type type) {
            return type.IsAbstract;
        }

        public static bool IsInterface(this Type type) {
            return type.IsInterface;
        }

        public static bool IsPrimitive(this Type type) {
            return type.IsPrimitive;
        }

        public static bool IsValueType(this Type type) {
            return type.IsValueType;
        }

        public static bool IsGenericType(this Type type) {
            return type.IsGenericType;
        }

        public static bool IsGenericParameter(this Type type) {
            return type.IsGenericParameter;
        }

        public static bool IsGenericTypeDefinition(this Type type) {
            return type.IsGenericTypeDefinition;
        }

        public static Type BaseType(this Type type) {
            return type.BaseType;
        }

        public static Assembly Assembly(this Type type) {
            return type.Assembly;
        }
    }
}

#endif