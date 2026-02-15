using Products.Api.Application.DTOs.Outputs.ProductDetail;
using Products.Api.Application.DTOs.Outputs.Products;

namespace Products.Api.Application.Helpers;

/// <summary>
/// Helper para enriquecer un ProductDetailOutput básico con datos simulados
/// de un marketplace completo. En producción, estos datos vendrían de múltiples
/// servicios/repositorios.
/// </summary>
public static class ProductEnricherHelper
{
    /// <summary>
    /// Enriquece un producto básico con toda la información necesaria
    /// para una página de detalle de marketplace.
    /// </summary>
    public static ProductDetailEnrichedOutput EnrichProduct(ProductDetailOutput basicProduct)
    {
        var random = new Random((int)basicProduct.Id); // Seed para consistencia
        
        return new ProductDetailEnrichedOutput
        {
            // Datos base del producto existente
            Id = basicProduct.Id,
            Name = basicProduct.Name,
            Description = basicProduct.Description,
            Sku = GenerateSku(basicProduct),
            Condition = "new",
            
            // Precio enriquecido
            Price = GeneratePriceInfo(basicProduct.Price, random),
            
            // Stock enriquecido
            Stock = GenerateStockInfo(basicProduct.Stock),
            
            // Imágenes simuladas
            Images = GenerateImages(basicProduct.Id, basicProduct.Name),
            
            // Categoría enriquecida
            Category = new CategoryInfoOutput
            {
                Id = basicProduct.Category.Id,
                Name = basicProduct.Category.Name,
                IconUrl = $"https://cdn.marketplace.com/icons/category-{basicProduct.Category.Id}.svg"
            },
            
            // Breadcrumbs de navegación
            Breadcrumbs = GenerateBreadcrumbs(basicProduct.Category),
            
            // Vendedor simulado
            Seller = GenerateSellerInfo(basicProduct.Id, random),
            
            // Atributos técnicos
            Attributes = GenerateAttributes(basicProduct),
            
            // Variantes (simuladas)
            Variants = GenerateVariants(basicProduct, random),
            
            // Información de envío
            Shipping = GenerateShippingInfo(random),
            
            // Rating y reviews
            Rating = GenerateRatingInfo(random),
            QuestionsCount = random.Next(5, 150),
            ReviewsCount = random.Next(10, 500),
            
            // Garantía
            Warranty = GenerateWarrantyInfo(),
            
            // Productos relacionados (IDs simulados)
            RelatedProducts = GenerateRelatedProducts(basicProduct.Id, random),
            
            // Metadatos
            CreatedAt = DateTime.UtcNow.AddDays(-random.Next(30, 365)),
            UpdatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
            Permalink = $"https://marketplace.com/product/{basicProduct.Id}/{Slugify(basicProduct.Name)}"
        };
    }
    
    private static string GenerateSku(ProductDetailOutput product)
    {
        return $"SKU-{product.Category.Id:D3}-{product.Id:D6}";
    }
    
    private static PriceInfoOutput GeneratePriceInfo(decimal price, Random random)
    {
        var hasDiscount = random.Next(100) < 30; // 30% de productos con descuento
        var discountPercentage = hasDiscount ? random.Next(10, 50) : (int?)null;
        var originalPrice = hasDiscount ? price * (1 + discountPercentage!.Value / 100m) : (decimal?)null;
        
        return new PriceInfoOutput
        {
            Amount = price,
            Currency = "ARS",
            OriginalAmount = originalPrice.HasValue ? Math.Round(originalPrice.Value, 2) : null,
            DiscountPercentage = discountPercentage,
            PaymentMethods = new List<PaymentMethodOutput>
            {
                new()
                {
                    Type = "credit_card",
                    Name = "Visa, Mastercard, American Express",
                    Installments = 12,
                    InstallmentAmount = Math.Round(price / 12, 2),
                    InterestFree = price > 50000
                },
                new()
                {
                    Type = "debit_card",
                    Name = "Visa Débito, Maestro",
                    Installments = 1,
                    InstallmentAmount = price,
                    InterestFree = true
                },
                new()
                {
                    Type = "transfer",
                    Name = "Transferencia bancaria",
                    Installments = 1,
                    InstallmentAmount = price * 0.95m, // 5% descuento
                    InterestFree = true
                }
            }
        };
    }
    
    private static StockInfoOutput GenerateStockInfo(int stock)
    {
        return new StockInfoOutput
        {
            AvailableQuantity = stock,
            Status = stock switch
            {
                0 => "out_of_stock",
                <= 5 => "last_units",
                _ => "available"
            },
            MaxPurchaseQuantity = Math.Min(stock, 6)
        };
    }
    
    private static List<ProductImageOutput> GenerateImages(long productId, string productName)
    {
        return Enumerable.Range(1, 5).Select(i => new ProductImageOutput
        {
            Id = $"img-{productId}-{i}",
            Url = $"https://cdn.marketplace.com/products/{productId}/image-{i}.jpg",
            ThumbnailUrl = $"https://cdn.marketplace.com/products/{productId}/thumb-{i}.jpg",
            Order = i,
            IsPrimary = i == 1,
            AltText = $"{productName} - Imagen {i}"
        }).ToList();
    }
    
    private static List<BreadcrumbOutput> GenerateBreadcrumbs(
        Application.DTOs.Outputs.Categories.CategoryOutput category)
    {
        return new List<BreadcrumbOutput>
        {
            new() { Id = 1, Name = "Inicio", Level = 0 },
            new() { Id = 100, Name = "Categorías", Level = 1 },
            new() { Id = category.Id, Name = category.Name, Level = 2 }
        };
    }
    
    private static SellerInfoOutput GenerateSellerInfo(long productId, Random random)
    {
        var sellerId = (productId % 10) + 1; // Simular diferentes vendedores
        var sellerNames = new[] 
        { 
            "TechStore Oficial", "ElectroMax", "GadgetWorld", "MegaShop", 
            "Digital Express", "SmartBuy", "TecnoPlus", "InnovaStore",
            "PrimeDeals", "SuperTech"
        };
        
        var levels = new[] { "yellow", "orange", "green", "gold", "platinum" };
        
        return new SellerInfoOutput
        {
            Id = sellerId,
            Name = sellerNames[sellerId - 1],
            LogoUrl = $"https://cdn.marketplace.com/sellers/{sellerId}/logo.png",
            IsOfficialStore = random.Next(100) < 20,
            YearsInPlatform = random.Next(1, 10),
            Reputation = new SellerReputationOutput
            {
                Level = levels[random.Next(levels.Length)],
                TotalSales = random.Next(100, 50000),
                PositiveRating = (decimal)Math.Round(85 + random.NextDouble() * 15, 1),
                CompletedTransactions = random.Next(50, 10000)
            },
            Location = new SellerLocationOutput
            {
                City = "Buenos Aires",
                State = "Buenos Aires",
                Country = "Argentina"
            }
        };
    }
    
    private static List<ProductAttributeOutput> GenerateAttributes(ProductDetailOutput product)
    {
        var attributes = new List<ProductAttributeOutput>
        {
            new() { Id = "brand", Name = "Marca", Value = "Generic Brand", Group = "main" },
            new() { Id = "model", Name = "Modelo", Value = $"Model-{product.Id}", Group = "main" },
            new() { Id = "sku", Name = "SKU", Value = $"SKU-{product.Id:D6}", Group = "main" },
            new() { Id = "weight", Name = "Peso", Value = "500", Unit = "g", Group = "dimensions" },
            new() { Id = "height", Name = "Alto", Value = "10", Unit = "cm", Group = "dimensions" },
            new() { Id = "width", Name = "Ancho", Value = "15", Unit = "cm", Group = "dimensions" },
            new() { Id = "depth", Name = "Profundidad", Value = "5", Unit = "cm", Group = "dimensions" }
        };
        
        // Agregar atributos específicos por categoría
        if (product.Category.Name.Contains("Electr", StringComparison.OrdinalIgnoreCase))
        {
            attributes.Add(new() { Id = "voltage", Name = "Voltaje", Value = "220", Unit = "V", Group = "electrical" });
            attributes.Add(new() { Id = "power", Name = "Potencia", Value = "50", Unit = "W", Group = "electrical" });
        }
        
        return attributes;
    }
    
    private static List<ProductVariantOutput> GenerateVariants(ProductDetailOutput product, Random random)
    {
        // Simular variantes solo si el producto podría tenerlas
        if (product.Stock < 5) return new List<ProductVariantOutput>();
        
        var colors = new[] { "Negro", "Blanco", "Azul", "Rojo" };
        var selectedColors = colors.Take(random.Next(1, 4)).ToList();
        
        return selectedColors.Select((color, index) => new ProductVariantOutput
        {
            Id = product.Id * 100 + index,
            Sku = $"SKU-{product.Id:D6}-{color[..1]}",
            Price = product.Price + (index * 100), // Ligera variación de precio
            Stock = Math.Max(1, product.Stock / selectedColors.Count),
            ThumbnailUrl = $"https://cdn.marketplace.com/products/{product.Id}/variant-{color.ToLower()}.jpg",
            Attributes = new List<VariantAttributeOutput>
            {
                new() { Name = "Color", Value = color }
            }
        }).ToList();
    }
    
    private static ShippingInfoOutput GenerateShippingInfo(Random random)
    {
        var freeShipping = random.Next(100) < 40; // 40% envío gratis
        
        return new ShippingInfoOutput
        {
            FreeShipping = freeShipping,
            LocalPickupAvailable = random.Next(100) < 30,
            Origin = new ShippingOriginOutput
            {
                City = "Buenos Aires",
                State = "Buenos Aires"
            },
            Options = new List<ShippingOptionOutput>
            {
                new()
                {
                    Id = "standard",
                    Name = "Envío estándar",
                    Cost = freeShipping ? 0 : random.Next(500, 2000),
                    EstimatedDeliveryDays = random.Next(3, 7),
                    EstimatedDeliveryDate = DateTime.UtcNow.AddDays(random.Next(3, 7)).ToString("yyyy-MM-dd"),
                    Carrier = "Correo Argentino"
                },
                new()
                {
                    Id = "express",
                    Name = "Envío express",
                    Cost = random.Next(1500, 4000),
                    EstimatedDeliveryDays = random.Next(1, 3),
                    EstimatedDeliveryDate = DateTime.UtcNow.AddDays(random.Next(1, 3)).ToString("yyyy-MM-dd"),
                    Carrier = "Andreani"
                }
            }
        };
    }
    
    private static RatingSummaryOutput GenerateRatingInfo(Random random)
    {
        var totalReviews = random.Next(10, 500);
        
        // Distribución realista de ratings (sesgada hacia positivo)
        var dist = new Dictionary<int, int>
        {
            { 5, (int)(totalReviews * 0.55) },
            { 4, (int)(totalReviews * 0.25) },
            { 3, (int)(totalReviews * 0.12) },
            { 2, (int)(totalReviews * 0.05) },
            { 1, (int)(totalReviews * 0.03) }
        };
        
        // Calcular promedio
        var weightedSum = dist.Sum(kvp => kvp.Key * kvp.Value);
        var average = totalReviews > 0 ? (decimal)weightedSum / totalReviews : 0;
        
        return new RatingSummaryOutput
        {
            Average = Math.Round(average, 1),
            TotalReviews = totalReviews,
            Distribution = dist
        };
    }
    
    private static WarrantyInfoOutput GenerateWarrantyInfo()
    {
        return new WarrantyInfoOutput
        {
            Type = "seller",
            DurationMonths = 12,
            Description = "Garantía del vendedor por defectos de fabricación"
        };
    }
    
    private static List<ProductSummaryOutput> GenerateRelatedProducts(long productId, Random random)
    {
        // Generar 4-6 productos relacionados (IDs diferentes al actual)
        var relatedCount = random.Next(4, 7);
        
        return Enumerable.Range(1, relatedCount)
            .Select(i =>
            {
                var relatedId = ((productId + i) % 100) + 1;
                return new ProductSummaryOutput
                {
                    Id = relatedId,
                    Name = $"Producto Relacionado {relatedId}",
                    Price = random.Next(1000, 50000),
                    ThumbnailUrl = $"https://cdn.marketplace.com/products/{relatedId}/thumb-1.jpg",
                    Rating = Math.Round(3.5m + (decimal)random.NextDouble() * 1.5m, 1),
                    FreeShipping = random.Next(100) < 40
                };
            })
            .ToList();
    }
    
    private static string Slugify(string text)
    {
        return text
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("á", "a")
            .Replace("é", "e")
            .Replace("í", "i")
            .Replace("ó", "o")
            .Replace("ú", "u")
            .Replace("ñ", "n");
    }
}

