using NetworkHelpers.Packages;

namespace NetworkHelpers.Abstractions;

public interface ISpecificPackage
{
    Package ConvertToPackage();
}
