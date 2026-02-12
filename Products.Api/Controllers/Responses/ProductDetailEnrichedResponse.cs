namespace Products.Api.Controllers.Responses;

/// <summary>
/// Respuesta enriquecida para la página de detalle de producto estilo marketplace.
/// Esta estructura simula la información completa que necesitaría un frontend
/// para renderizar una página de detalle similar a MercadoLibre/Amazon.
/// </summary>
public class ProductDetailEnrichedResponse
{
    // === Información básica del producto ===
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string Condition { get; set; } = "new"; // "new", "used", "refurbished"
    
    // === Precios ===
    public PriceInfoResponse Price { get; set; } = new();
    
    // === Stock y disponibilidad ===
    public StockInfoResponse Stock { get; set; } = new();
    
    // === Multimedia ===
    public List<ProductImageResponse> Images { get; set; } = new();
    
    // === Categorización ===
    public CategoryInfoResponse Category { get; set; } = new();
    public List<BreadcrumbResponse> Breadcrumbs { get; set; } = new();
    
    // === Vendedor ===
    public SellerInfoResponse Seller { get; set; } = new();
    
    // === Atributos técnicos ===
    public List<ProductAttributeResponse> Attributes { get; set; } = new();
    
    // === Variantes (talla, color, etc.) ===
    public List<ProductVariantResponse> Variants { get; set; } = new();
    
    // === Envío ===
    public ShippingInfoResponse Shipping { get; set; } = new();
    
    // === Social proof ===
    public RatingSummaryResponse Rating { get; set; } = new();
    public int QuestionsCount { get; set; }
    public int ReviewsCount { get; set; }
    
    // === Garantía ===
    public WarrantyInfoResponse? Warranty { get; set; }
    
    // === Productos relacionados ===
    public List<ProductSummaryResponse> RelatedProducts { get; set; } = new();
    
    // === Metadatos ===
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Permalink { get; set; } = string.Empty;
}

public class PriceInfoResponse
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ARS";
    public decimal? OriginalAmount { get; set; } // Para mostrar descuentos
    public int? DiscountPercentage { get; set; }
    public List<PaymentMethodResponse> PaymentMethods { get; set; } = new();
}

public class PaymentMethodResponse
{
    public string Type { get; set; } = string.Empty; // "credit_card", "debit_card", "cash", "transfer"
    public string Name { get; set; } = string.Empty;
    public int? Installments { get; set; }
    public decimal? InstallmentAmount { get; set; }
    public bool InterestFree { get; set; }
}

public class StockInfoResponse
{
    public int AvailableQuantity { get; set; }
    public string Status { get; set; } = "available"; // "available", "last_units", "out_of_stock"
    public int? MaxPurchaseQuantity { get; set; }
}

public class ProductImageResponse
{
    public string Id { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public int Order { get; set; }
    public bool IsPrimary { get; set; }
    public string? AltText { get; set; }
}

public class CategoryInfoResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
}

public class BreadcrumbResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
}

public class SellerInfoResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public SellerReputationResponse Reputation { get; set; } = new();
    public SellerLocationResponse Location { get; set; } = new();
    public bool IsOfficialStore { get; set; }
    public int YearsInPlatform { get; set; }
}

public class SellerReputationResponse
{
    public string Level { get; set; } = "gold"; // "newbie", "yellow", "orange", "green", "gold", "platinum"
    public int TotalSales { get; set; }
    public decimal PositiveRating { get; set; } // Porcentaje 0-100
    public int CompletedTransactions { get; set; }
}

public class SellerLocationResponse
{
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = "Argentina";
}

public class ProductAttributeResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public string Group { get; set; } = "main"; // Para agrupar atributos en secciones
}

public class ProductVariantResponse
{
    public long Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public List<VariantAttributeResponse> Attributes { get; set; } = new();
    public string? ThumbnailUrl { get; set; }
}

public class VariantAttributeResponse
{
    public string Name { get; set; } = string.Empty; // "Color", "Talla", "Capacidad"
    public string Value { get; set; } = string.Empty; // "Rojo", "XL", "128GB"
}

public class ShippingInfoResponse
{
    public bool FreeShipping { get; set; }
    public List<ShippingOptionResponse> Options { get; set; } = new();
    public ShippingOriginResponse Origin { get; set; } = new();
    public bool LocalPickupAvailable { get; set; }
}

public class ShippingOptionResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty; // "Envío estándar", "Envío express"
    public decimal Cost { get; set; }
    public int EstimatedDeliveryDays { get; set; }
    public string? EstimatedDeliveryDate { get; set; }
    public string Carrier { get; set; } = string.Empty;
}

public class ShippingOriginResponse
{
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
}

public class RatingSummaryResponse
{
    public decimal Average { get; set; } // 0.0 - 5.0
    public int TotalReviews { get; set; }
    public Dictionary<int, int> Distribution { get; set; } = new(); // { 5: 100, 4: 50, 3: 20, 2: 5, 1: 2 }
}

public class WarrantyInfoResponse
{
    public string Type { get; set; } = string.Empty; // "seller", "manufacturer", "extended"
    public int DurationMonths { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class ProductSummaryResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ThumbnailUrl { get; set; } = string.Empty;
    public decimal? Rating { get; set; }
    public bool FreeShipping { get; set; }
}
