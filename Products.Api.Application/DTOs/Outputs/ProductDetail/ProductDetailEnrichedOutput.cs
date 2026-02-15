namespace Products.Api.Application.DTOs.Outputs.ProductDetail;

public class ProductDetailEnrichedOutput
{
    // === Información básica del producto ===
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string Condition { get; set; } = "new"; // "new", "used", "refurbished"
    
    // === Precios ===
    public PriceInfoOutput Price { get; set; } = new();
    
    // === Stock y disponibilidad ===
    public StockInfoOutput Stock { get; set; } = new();
    
    // === Multimedia ===
    public List<ProductImageOutput> Images { get; set; } = new();
    
    // === Categorización ===
    public CategoryInfoOutput Category { get; set; } = new();
    public List<BreadcrumbOutput> Breadcrumbs { get; set; } = new();
    
    // === Vendedor ===
    public SellerInfoOutput Seller { get; set; } = new();
    
    // === Atributos técnicos ===
    public List<ProductAttributeOutput> Attributes { get; set; } = new();
    
    // === Variantes (talla, color, etc.) ===
    public List<ProductVariantOutput> Variants { get; set; } = new();
    
    // === Envío ===
    public ShippingInfoOutput Shipping { get; set; } = new();
    
    // === Social proof ===
    public RatingSummaryOutput Rating { get; set; } = new();
    public int QuestionsCount { get; set; }
    public int ReviewsCount { get; set; }
    
    // === Garantía ===
    public WarrantyInfoOutput? Warranty { get; set; }
    
    // === Productos relacionados ===
    public List<ProductSummaryOutput> RelatedProducts { get; set; } = new();
    
    // === Metadatos ===
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Permalink { get; set; } = string.Empty;
}

public class PriceInfoOutput
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ARS";
    public decimal? OriginalAmount { get; set; } // Para mostrar descuentos
    public int? DiscountPercentage { get; set; }
    public List<PaymentMethodOutput> PaymentMethods { get; set; } = new();
}

public class PaymentMethodOutput
{
    public string Type { get; set; } = string.Empty; // "credit_card", "debit_card", "cash", "transfer"
    public string Name { get; set; } = string.Empty;
    public int? Installments { get; set; }
    public decimal? InstallmentAmount { get; set; }
    public bool InterestFree { get; set; }
}

public class StockInfoOutput
{
    public int AvailableQuantity { get; set; }
    public string Status { get; set; } = "available"; // "available", "last_units", "out_of_stock"
    public int? MaxPurchaseQuantity { get; set; }
}

public class ProductImageOutput
{
    public string Id { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsPrimary { get; set; }
    public string? AltText { get; set; }
}

public class CategoryInfoOutput
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
}

public class BreadcrumbOutput
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
}

public class SellerInfoOutput
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public SellerReputationOutput Reputation { get; set; } = new();
    public SellerLocationOutput Location { get; set; } = new();
    public bool IsOfficialStore { get; set; }
    public int YearsInPlatform { get; set; }
}

public class SellerReputationOutput
{
    public string Level { get; set; } = "gold"; // "newbie", "yellow", "orange", "green", "gold", "platinum"
    public int TotalSales { get; set; }
    public decimal PositiveRating { get; set; } // Porcentaje 0-100
    public int CompletedTransactions { get; set; }
}

public class SellerLocationOutput
{
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = "Argentina";
}

public class ProductAttributeOutput
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public string Group { get; set; } = "main"; // Para agrupar atributos en secciones
}

public class ProductVariantOutput
{
    public long Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public List<VariantAttributeOutput> Attributes { get; set; } = new();
    public string? ThumbnailUrl { get; set; }
}

public class VariantAttributeOutput
{
    public string Name { get; set; } = string.Empty; // "Color", "Talla", "Capacidad"
    public string Value { get; set; } = string.Empty; // "Rojo", "XL", "128GB"
}

public class ShippingInfoOutput
{
    public bool FreeShipping { get; set; }
    public List<ShippingOptionOutput> Options { get; set; } = new();
    public ShippingOriginOutput Origin { get; set; } = new();
    public bool LocalPickupAvailable { get; set; }
}

public class ShippingOptionOutput
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty; // "Envío estándar", "Envío express"
    public decimal Cost { get; set; }
    public int EstimatedDeliveryDays { get; set; }
    public string? EstimatedDeliveryDate { get; set; }
    public string Carrier { get; set; } = string.Empty;
}

public class ShippingOriginOutput
{
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
}

public class RatingSummaryOutput
{
    public decimal Average { get; set; } // 0.0 - 5.0
    public int TotalReviews { get; set; }
    public Dictionary<int, int> Distribution { get; set; } = new(); // { 5: 100, 4: 50, 3: 20, 2: 5, 1: 2 }
}

public class WarrantyInfoOutput
{
    public string Type { get; set; } = string.Empty; // "seller", "manufacturer", "extended"
    public int DurationMonths { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class ProductSummaryOutput
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ThumbnailUrl { get; set; } = string.Empty;
    public decimal? Rating { get; set; }
    public bool FreeShipping { get; set; }
}
