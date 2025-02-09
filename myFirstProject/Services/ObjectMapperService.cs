using System;
using System.Reflection;

namespace myFirstProject.Services
{
    public interface IObjectMapperService
    {
        TDestination Map<TSource, TDestination>(TSource source) 
            where TDestination : new();
    }

    public class ObjectMapperService : IObjectMapperService
    {
        public TDestination Map<TSource, TDestination>(TSource source) 
            where TDestination : new()
        {
            var destination = new TDestination();
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);

            var sourceProperties = sourceType.GetProperties();
            var destinationProperties = destinationType.GetProperties();

            foreach (var destProperty in destinationProperties)
            {
                var attr = destProperty.GetCustomAttribute<MapFromAttribute>();
                
                var sourcePropertyName = attr?.SourceProperty ?? destProperty.Name;
                
                PropertyInfo sourceProperty = null;
                foreach (var sp in sourceProperties)
                {
                    if (sp.Name == sourcePropertyName)
                    {
                        sourceProperty = sp;
                        break;
                    }
                }
                
                if (sourceProperty == null)
                    continue;
                MapProperty(source, destination, sourceProperty, destProperty);
            }

            return destination;
        }

        private void MapProperty<TSource, TDestination>(
            TSource source, 
            TDestination destination,
            PropertyInfo sourceProperty,
            PropertyInfo destProperty)
        {
            try
            {
                var sourceValue = sourceProperty.GetValue(source);
                
                if (sourceValue == null) 
                    return;
                
                var destType = Nullable.GetUnderlyingType(destProperty.PropertyType) 
                    ?? destProperty.PropertyType;
                var convertedValue = Convert.ChangeType(sourceValue, destType);
                
                destProperty.SetValue(destination, convertedValue);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Error mapping {sourceProperty.Name} to {destProperty.Name}", ex);
            }
        }
    }

    
    [AttributeUsage(AttributeTargets.Property)]
    public class MapFromAttribute : Attribute
    {
        public string SourceProperty { get; }

        public MapFromAttribute(string sourceProperty)
        {
            SourceProperty = sourceProperty;
        }
    }
}
