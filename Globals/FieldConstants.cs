namespace Corno.Web.Globals;

public static class FieldConstants
{
    // Contexts
    public const string CornoContext = "CornoContext";
    public const string WccContext = "WccContext";
    public const string ImosContext = "ImosContext";
    public const string ImosBodContext = "ImosBodContext";

    // Project 
    public const string Project = "Project";
    public const string Projects = "Projects";
    public const string ProjectNo = "ProjectNo";
    public const string ProjectCode = "ProjectCode";
    public const string ProjectId = "ProjectId";
    public const string ProjectWbsElement = "ProjectWbsElement";

    // Company
    public const string Company = "Company";
    public const string CompanyId = "CompanyId";
    public const string FinancialYearId = "FinancialYearId";
    public const string FinancialYear = "FinancialYear";

    // Masters 
    public const string SerialNo = "SerialNo";
    public const string Code = "Code";
    public const string Id = "Id";
    public const string Name = "Name";
    public const string NameWithCode = "NameWithCode";
    public const string NameWithId = "NameWithId";
    public const string Description = "Description";
    public const string SubDescription = "SubDescription";
    public const string ProjectWBSElement = "ProjectWBSElement";
    public const string ShortText = "ShortText";
    public const string NoOfPackages = "NoOfPackages";
    public const string IdoNo = "IdoNo";
    public const string PackingNo = "PackingNo";
    public const string DocketNo = "DocketNo";
    public const string CoilNo = "CoilNo";
    public const string SetNo = "SetNo";
    public const string NoOfBoxes = "NoOfBoxes";
    public const string Nominated = "Nominated";
    public const string Status = "Status";

    // General
    public const string NA = "NA";
    public const string Caption = "Caption";

    // Import / Export
    public const string Import = "Import";
    public const string ImportedPath = "ImportedPath";
    public const string UnImportedPath = "UnImportedPath";
    public const string Export = "Export";
    public const string Imported = "Imported";
    public const string Ignored = "Ignored";
    public const string Exist = "Exist";


    // GRN
    public const string UnitId = "UnitId";
    public const string Unit = "Unit";
    public const string WrtUnitId = "WrtUnitId";
    public const string WrtUnit = "WRT Unit";
    public const string OrderUnit = "OrderUnit";
    public const string DeliveryNo = "DeliveryNo";

    public const string Duplicate = "Duplicate";

    public const string BatchLabelQuantity = "BatchLabelQuantity";
    public const string LabelQuantity = "LabelQuantity";
    public const string PendingQuantity = "PendingQuantity";
    public const string PacketQuantity = "PacketQuantity";
    public const string IdoQuantity = "IdoQuantity";
    public const string PackingQuantity = "PackingQuantity";
    public const string OdoQuantity = "OdoQuantity";
    public const string SystemWeight = "SystemWeight";
    public const string ActualWeight = "ActualWeight";
    public const string Variance = "Variance";
    public const string BinNo = "BinNo";
    public const string BatchCode = "BatchCode";
    public const string ScanDate = "ScanDate";
    public const string PlanDate = "PlanDate";
    public const string OdoDate = "OdoDate";
    public const string BagWeight = "BagWeight";

    // All Standard - User Related
    public const string UserId = "UserId";
    public const string UserName = "UserName";
    public const string User = "User";
    public const string CreatedBy = "CreatedBy";
    public const string CreatedDate = "CreatedDate";
    public const string ModifiedBy = "ModifiedBy";
    public const string ModifiedDate = "ModifiedDate";
    public const string DeletedBy = "DeletedBy";
    public const string DeletedDate = "DeletedDate";
    public const string DeleteFile = "DeleteFile";
    public const string MoveFile = "MoveFile";

    // Item
    public const string Group = "Group";
    public const string ItemCode = "ItemCode";
    public const string ParentItemCode = "ParentItemCode";
    public const string ParentItems = "ParentItems";
    public const string ItemName = "ItemName";
    public const string Item = "Item";
    public const string Items = "Items";
    public const string ItemType = "ItemType";
    public const string ItemId = "ItemId";
    public const string ItemIds = "ItemIds";
    public const string CourierId = "CourierId";
    public const string MachineType = "Machine Type";
    public const string MachineTypeId = "MachineTypeId";
    public const string ProductCode = "ProductCode";
    public const string ProductType = "Product Type";
    public const string ProductGroup = "ProductGroup";
    public const string ProductFamily = "Product Family";
    public const string Family = "Family";
    public const string Families = "Families";
    public const string ProductTypeId = "ProductTypeId";
    public const string ProductFamilyId = "ProductFamilyId";
    public const string ProductCategoryId = "ProductCategoryId";
    public const string ProductCategory = "Product Category";
    public const string ConsumableType = "Consumable Type";
    public const string ConsumableTypeId = "ConsumableTypeId";
    public const string Machine = "Machine";
    public const string MachineId = "MachineId";
    public const string MachineNo = "MachineNo";
    public const string MachineCode = "MachineCode"; 
    public const string Assembly = "Assembly";
    public const string SubAssembly = "SubAssembly";
    public const string SubAssemblyId = "SubAssemblyId";
    public const string SubAssemblyItems = "SubAssemblyItems";
    public const string Cell = "Cell";
    public const string CellId = "CellId";
    public const string EmployeeType = "Employee Type";
    public const string EmployeeTypeId = "EmployeeTypeId";
    public const string Quantity = "Quantity";
    public const string UserQuantity = "UserQuantity";
    public const string MaxQuantity = "MaxQuantity";
    public const string PlanQuantity = "PlanQuantity";
    public const string LoadQuantity = "LoadQuantity";
    public const string BalanceQuantity = "BalanceQuantity";
    public const string ActualQuantity = "ActualQuantity";
    public const string PlannedQuantity = "PlannedQuantity";
    public const string IssuedQuantity = "IssuedQuantity";
    public const string Rate = "Rate";
    public const string Amount = "Amount";
    public const string DiscountPercent = "DiscountPercent";
    public const string DiscountAmount = "DiscountAmount";
    public const string Cgst = "Cgst";
    public const string Sgst = "Sgst";
    public const string Igst = "Igst";
    public const string Hsn = "Hsn";
    public const string CutQuantity = "CutQuantity";
    public const string BomQuantity = "BomQuantity";
    public const string PackedQuantity = "PackedQuantity";
    public const string PackQuantity = "PackQuantity";
    public const string StandardWeight = "StandardWeight";
    public const string ScannedQuantity = "ScannedQuantity";
    public const string ComponentId = "ComponentId";
    public const string Component = "Component";
    public const string ProductId = "ProductId";
    public const string Product = "Product";
    public const string Products = "Products";
    public const string SkuId = "SkuId";
    public const string Sku = "Sku";
    public const string ReasonCodeId = "ReasonCodeId";
    public const string ReasonCode = "ReasonCode";
    public const string MiscType = "MiscType";

    public const string Cutting = "Cutting";
    public const string Lipping = "Lipping";
    public const string Curveflowing = "Curveflowing";
    public const string Drilling = "Drilling";
    public const string Packing = "Packing";

    public const string Compound = "Compound";
    public const string CompoundId = "CompoundId";
    public const string StationId = "StationId";
    public const string Station = "Station";
    public const string Phr = "Phr";
    public const string Multiple = "Multiple";
    public const string Divide = "Divide";
    public const string Weight = "Weight";

    public const string NoOfStrokes = "NoOfStrokes";
    public const string ToolLifeInStrokes = "ToolLifeInStrokes";
    public const string UsedStrokes = "UsedStrokes";
    public const string RequiredStrokes = "RequiredStrokes";

    public const string Ok = "OK";
    public const string Rework = "Rework";
    public const string Process = "Process";
    public const string Recut = "Recut";

    public const string LineId = "LineId";
    public const string Line = "Line";
    public const string PackingLineId = "PackingLineId";
    public const string PackingLine = "LinePackingLine";
    public const string PackingList = "PackingList";
    public const string PackingLists = "PackingLists";
    public const string PackingListNo = "PackingListNo";

    // Accounting
    public const string TaxCode = "TaxCode";
    public const string Currency = "Currency";

    // Departments 
    public const string AttendanceDate = "AttendanceDate";
    public const string ManPowerAllocationDate = "ManPowerAllocationDate";
    public const string ProductionDate = "ProductionDate";
    public const string ScrapDate = "ScrapDate";
    public const string ConsumableDate = "ConsumableDate";
    public const string DepartmentId = "DepartmentId";

    // General
    public const string Customer = "Customer";
    public const string CustomerCode = "CustomerCode";
    public const string CustomerName = "CustomerName";
    public const string ProjectInChargeName = "ProjectInChargeName";
    public const string SoldToCountry = "SoldToCountry";
    public const string TypeOfProject = "TypeOfProject";
    public const string CustomerId = "CustomerId";
    public const string Supplier = "Supplier";
    public const string SupplierId = "SupplierId";
    public const string SupplierCode = "SupplierCode";
    public const string SupplierName = "SupplierName";
    public const string SupplierDescription = "SupplierDescription";
    public const string ContactDetails = "ContactDetails";

    // Price
    public const string Price1 = "Price1";
    public const string Price2 = "Price2";
    public const string Price3 = "Price3";
    public const string NetPrice = "NetPrice";

    public const string Estimate = "Estimate";
    public const string EstimateId = "EstimateId";
    public const string Design = "Design";
    public const string DesignId = "DesignId";
    public const string Dispatch = "Dispatch";
    public const string DispatchId = "DispatchId";
    public const string Grn = "Grn";
    public const string GrnNo = "GrnNo";
    public const string GrnDate = "GrnDate";
    public const string GrnTxnCode = "GrnTxnCode";
    public const string MaterialInwardId = "MaterialInwardId";
    public const string Inward = "Inward";
    public const string InwardId = "InwardId";
    public const string InwardInspection = "InwardInspection";
    public const string InwardInspectionId = "InwardInspectionId";
    public const string LinkedEstimateId = "LinkedEstimateId";
    public const string NetWeight = "NetWeight";
    public const string EmptyBoxWeight = "EmptyBoxWeight";
    public const string Tolerance = "Tolerance";
    public const string Mrp = "MRP";
    public const string WeightPerPiece = "WeightPerPiece";
    public const string TareWeight = "TareWeight";
    public const string GrossWeight = "GrossWeight";
    public const string BundleWeight = "BundleWeight";
    public const string WeightPerSet = "WeightPerSet";
    public const string WeightDifference = "WeightDifference";
    
    public const string FreshPieces = "FreshPieces";
    public const string FreshNetWeight = "FreshNetWeight";
    public const string FreshWeightPerPiece = "FreshWeightPerPiece";
    public const string SecondsPieces = "SecondsPieces";
    public const string SecondsNetWeight = "SecondsNetWeight";
    public const string SecondsWeightPerPiece = "SecondsWeightPerPiece";
    public const string ThirdsPieces = "ThirdsPieces";
    public const string ThirdsNetWeight = "ThirdsNetWeight";
    public const string ThirdsWeightPerPiece = "ThirdsWeightPerPiece";
    public const string RepolishPieces = "RepolishPieces";
    public const string RepolishNetWeight = "RepolishNetWeight";
    public const string RepolishWeightPerPiece = "RepolishWeightPerPiece";
    public const string TotalPieces = "TotalPieces";
    public const string TotalNetWeight = "TotalNetWeight";
    public const string TotalWeightPerPiece = "TotalWeightPerPiece";
    public const string Remark = "Remark";
    public const string Remarks = "Remarks";
    public const string DispatchWeight = "DispatchWeight";
    //public const string Unloading = "Unloading";
    //public const string PalletIn = "PalletIn";
    //public const string RackIn = "RackIn";
    //public const string RackOut = "RackOut";


    public const string Date = "Date";
    public const string ChallanDate = "ChallanDate";
    public const string EstimateDate = "EstimateDate";
    public const string DispatchDate = "DispatchDate";

    public const string FromDate = "FromDate";
    public const string ToDate = "ToDate";

    public const string ChallanNo = "ChallanNo";
    
    // Sales
    public const string SalesInvoice = "SalesInvoice";
    public const string SalesInvoices = "SalesInvoices";
    public const string SalesInvoiceId = "SalesInvoiceId";
    public const string InvoiceNo = "InvoiceNo";
    public const string InvoiceDate = "InvoiceDate";
    public const string SalesOrder = "SalesOrder";
    public const string SalesOrders = "SalesOrders";
    public const string SalesOrderId = "SalesOrderId";

    // Purchase
    public const string PurchaseReturn = "PurchaseReturn";
    public const string PurchaseReturnId = "PurchaseReturnId";
    public const string RejectedQuantity = "RejectedQuantity";

    // Documents
    public const string DocumentType = "DocumentType";
    public const string BusinessType = "BusinessType";

    // Vehicle
    public const string TypeofVehicle = "TypeofVehicle";
    public const string VehicleNo = "VehicleNo";
    public const string LrNo = "LrNo";

    // Location
    public const string LocationBarcode = "LocationBarcode";
    public const string LocationId = "LocationId";
    public const string Location = "Location";
    public const string PickUpLocation = "PickUpLocation";

    public const string PolishBook = "PolishBook";
    public const string PolishBookId = "PolishBookId";

    public const string MixingSequence = "MixingSequence";
    public const string WeighingSequence = "WeighingSequence";

    public const string Barcode = "Barcode";
    public const string Barcodes = "Barcodes";
    public const string MaterialCode = "MaterialCode";
    public const string FinishLength = "FinishLength";
    public const string FinishWidth = "FinishWidth";
    public const string Thickness = "Thickness";
    public const string ProductionRoute = "ProductionRoute";

    public const string Inspection = "Inspection";
    public const string InspectionId = "InspectionId";

    public const string General = "General";
    public const string Shift1 = "Shift1";
    public const string Shift2 = "Shift2";
    public const string Shift3 = "Shift3";
    public const string Total = "Total";

    public const string StartTime = "StartTime";
    public const string EndTime = "EndTime";

    public const string Relationship = "Relationship";
    public const string ReleaseDate = "ReleaseDate";
    public const string PlanningDate = "PlanningDate";
    public const string Plan = "Plan";
    public const string PlanItemDetail = "PlanItemDetail";
    public const string PlanId = "PlanId";
    public const string MomDate = "MomDate";
    public const string PlanNo = "PlanNo";
    public const string PlanningType = "PlanningType";
    public const string ReviewType = "ReviewType";
    public const string ReviewDate = "ReviewDate";
    public const string KittingDate = "KittingDate";
    public const string KittingRelease = "KittingRelease";
    public const string ManufacturingDueDate = "ManufacturingDueDate";
    public const string ManufacturingDate = "ManufacturingDate";
    public const string PaintingRelease = "PaintingRelease";
    public const string PaintingDate = "PaintingDate";
    public const string Apo = "Apo";
    public const string ReportingDate = "ReportingDate";
    public const string NsCategoryId = "NsCategoryId";
    public const string NsCategory = "NsCategory";
    public const string DrawingNo = "DrawingNo";
    public const string SellType = "SellType";
    public const string DueDate = "DueDate";
    public const string ProductBreifingDate = "ProductBreifingDate";
    public const string MockupDate = "MockupDate";
    public const string FinalDrawingsReleaseDate = "FinalDrawingsReleaseDate";
    public const string SourceId = "SourceId";
    public const string Source = "Source";
    public const string FreshPowderIn = "FreshPowderIn";
    public const string LoosePowderOut = "LoosePowderOut";
    public const string NetPowderUse = "NetPowderUse";
    public const string OtHr = "OtHr";
    public const string ProductionOrderNo = "ProductionOrderNo";
    public const string ParentPln = "ParentPln";
    public const string CildPln = "CildPln";
    public const string NoOfColorChange = "NoOfColorChange";
    public const string PaintShopQuantity = "PaintShopQuantity";
    public const string PaintShop13BQuantity = "PaintShop13BQuantity";
    public const string FinalFittingQuantity = "FinalFittingQuantity";
    public const string FinalFittingTime = "FinalFittingTime";
    public const string FinalFitting13BQuantity = "FinalFitting13BQuantity";
    public const string BendingAndAssemblyQuantity = "BendingAndAssemblyQuantity";
    public const string MetalFormingQuantity = "MetalFormingQuantity";
    public const string PedestalQuantity = "PedestalQuantity";

    // SMS
    public const string None = "None";
    public const string Once = "Once";
    public const string Yearly = "Yearly";
    public const string Monthly = "Monthly";
    public const string Weekly = "Weekly";
    public const string Daily = "Daily";
    public const string Hourly = "Hourly";
    public const string Minutely = "Minutely";

    // Departments
    public const string Administration = "Administration";

    // Transaction Types
    public const string Attendance = "Attendance";
    public const string Production = "Production";
    public const string Scrap = "Scrap";
    public const string IndexDate = "IndexDate";
    public const string Parameter = "Parameter";
    public const string ParameterId = "ParameterId";
    public const string ParameterType = "ParameterType";
    public const string MaxScore = "MaxScore";
    public const string DepartmentScore = "DepartmentScore";
    public const string IndexId = "IndexId";
    public const string IndexScore = "IndexScore";
    public const string StandardWeightage = "StandardWeightage";
    public const string DepartmentWeightage = "DepartmentWeightage";
    public const string IndexWeightage = "IndexWeightage";

    // Transport
    public const string TransportName = "TransportName";
    public const string TransporterName = "TransporterName";

    // Adam Boards
    public const string Adam1 = "Adam1";
    public const string Adam2 = "Adam2";
    public const string Adam3 = "Adam3";
    public const string Adam4 = "Adam4";
    public const string Adam5 = "Adam5";

    // Suyog Rubber
    public const string RecipeId = "RecipeId";
    public const string Fb = "FB";
    public const string Mb = "MB";
    public const string WeighingType = "WeighingType";
    public const string BatchType = "BatchType";

    // SPC
    public const string X1 = "X1";
    public const string X2 = "X2";
    public const string X3 = "X3";
    public const string X4 = "X4";
    public const string X5 = "X5";
    public const string Average = "Average";
    public const string Range = "Range";
    public const string Operator = "Operator";
    public const string OperatorId = "OperatorId";
    public const string RootCause = "RootCause";
    public const string Action = "Action";

    public const string OrderType = "OrderType";
    public const string InvoiceType = "InvoiceType";
    public const string ReturnType = "ReturnType";
    public const string OrderNo = "OrderNo";
    public const string PoNo = "PoNo";
    public const string PoDate = "PoDate";
    public const string SoNo = "SoNo";
    public const string SoDate = "SoDate";
    public const string SkuCode = "SkuCode";
    public const string SkuDescription = "SkuDescription";
    public const string CartonNo = "CartonNo";
    public const string CartonSerialNo = "CartonSerialNo";
    public const string PackingDate = "PackingDate";
    public const string Cartons = "Cartons";
    public const string CartonDetails = "CartonDetails";
    public const string CartonBarcode = "CartonBarcode";
    public const string BarcodeQuantity = "BarcodeQuantity";
    public const string Boxes = "Boxes";
    public const string Pieces = "Pieces";
    public const string BarcodeLabelSerialNo = "BarcodeLabelSerialNo";
    public const string WorkOrderSerialNo = "WorkOrderSerialNo";
    public const string GrnSerialNo = "GrnSerialNo";
    public const string AsnSerialNo = "AsnSerialNo";
    public const string AsnNo = "AsnNo";

    // Vikroli.Store
    public const string VendorEntry = "VendorEntry";
    public const string VendorEntryId = "VendorEntryId";
    public const string UnloadingAreaId = "UnloadingAreaId";
    public const string UnloadingArea = "Unloading Area";
    public const string Rejection = "Rejection";
    public const string RejectionId = "RejectionId";
    public const string RejectionDate = "RejectionDate";
    public const string MainGate1 = "MainGate1";
    public const string MainGate1Id = "MainGate1Id";
    public const string MainGate2 = "MainGate2";
    public const string MainGate2Id = "MainGate2Id";
    public const string Transaction = "Transaction";
    public const string TransactionId = "TransactionId";

    public const string Control = "Control";
    public const string Run = "Run";

    // cWallet
    public const string AddMoney = "Add Money";
    public const string Pay = "Pay";
    public const string Recieve = "Recieve";

    // HUL Traceability
    public const string ChemicalId = "ChemicalId";
    public const string Chemical = "Chemical";
    public const string WeighingId = "WeighingId";
    public const string TrackingNo = "TrackingNo";

    // Shirwal.Kitweigh
    public const string Wf = "Wf";
    public const string Baan = "Baan";
    public const string Infor = "Infor";
    public const string I4 = "I4";
    public const string Shirwal = "Shirwal";

    public const string IpAddress = "IpAddress";
    public const string Port = "Port";

    public const string Department = "Department";
    public const string From = "From";
    public const string To = "To";

    public const string Buffalo = "Buffalo";
    public const string Cow = "Cow";

    public const string Dimensions = "Dimensions";
    public const string LengthStartAddress = "LengthStartAddress";
    public const string WidthStartAddress = "WidthStartAddress";
    public const string ThicknessStartAddress = "ThicknessStartAddress";
    public const string DimensionStartAddress = "DimensionStartAddress";
    public const string ColorStartAddress = "ColorStartAddress";
    public const string WeightStartAddress = "WeightStartAddress";
    public const string AcceptStartAddress = "AcceptStartAddress";
    public const string InputSensorAddress = "InputSensorAddress";
    public const string InputEncoderAddress = "InputEncoderAddress";
    public const string OutputStartAddress = "OutputStartAddress";
    public const string DataStartAddress = "DataStartAddress";
    public const string TareCoilAddress = "TareCoilAddress";
    public const string IsWeightEnabled = "IsWeightEnabled";
    public const string IsMetalKitchen = "IsMetalKitchen";

    public const string Color = "Color";
    public const string NoOfLabels = "NoOfLabels";
    public const string BoxNo = "BoxNo";
    public const string PalletNo = "PalletNo";
    public const string RackNo = "RackNo";
    public const string PalletScanningDate = "PalletScanningDate";
    public const string BarcodeGenerationDate = "BarcodeGenerationDate";
    public const string RackId = "RackId";
    public const string RackScanningDate = "RackScanningDate";
    public const string RetrievalDate = "RetrievalDate";

    public const string Verification = "Verification";
    public const string Payment = "Payment";
    public const string StatusOfProject = "StatusOfProject";
    public const string MachineSpecification = "MachineSpecification";
    public const string StatusCheck = "StatusCheck";

    public const string Index = "Index";

    public const string Yes = "Yes";
    public const string No = "No";

    public const string Position = "Position";
    public const string Positions = "Positions";
    public const string Packet = "Packet";
    public const string Packets = "Packets";
    public const string PacketNo = "PacketNo";
    public const string PacketCode = "PacketCode";
    public const string MrpLabelPacket = "MrpLabelPacket";
    public const string CurrentProduction = "CurrentProduction";
    public const string ScanQuantity = "ScanQuantity";
    public const string ProductDescription = "ProductDescription";
    public const string SoPosition = "SoPosition";

    public const string WarehouseOrderNo = "WarehouseOrderNo";
    public const string WarehousePosition = "WarehousePosition";
    public const string RplNo = "RplNo";
    public const string RplPosition = "RplPosition";
    public const string WarehouseId = "WarehouseId";
    public const string WarehouseCode = "WarehouseCode";
    public const string Warehouse = "Warehouse";
    public const string Warehouses = "Warehouses";
    public const string BarcodeDayCounter = "BarcodeDayCounter";
    public const string LabelIndex = "LabelIndex";
    public const string LabelCount = "LabelCount";
    public const string Label = "Label";
    public const string Labels = "Labels";
    public const string LabelType = "LabelType";
    public const string LabelDate = "LabelDate";
    public const string Entities = "Entities";
    public const string Plans = "Plans";
    public const string TotalTime = "TotalTime";
    public const string LoadingStartTime = "LoadingStartTime";
    public const string LoadingEndTime = "LoadingEndTime";
    public const string LoadingSequence = "LoadingSequence";
    public const string PackingSequence = "PackingSequence";
    public const string QcCode = "QcCode";
    public const string LotNo = "LotNo";

    public const string PrinterName = "PrinterName";
    public const string RejectPrinter = "RejectPrinter";
    public const string AcceptPrinter = "AcceptPrinter";

    public const string Flavour = "Flavour";
    public const string FlavourId = "FlavourId";

    public const string Service = "Service";
    public const string UserType = "UserType";

    public const string PriorityId = "PriorityId";
    public const string Priority = "Priority";

    // 4Ever
    public const string Ever4KeyText = "7c3acb8b876hbd8a95b6a26d15029cf060444ec";
    public const string Iv = "9T3acb8b8uikuyesgteslaodb";
    public const string SessionIv = "8T3acb8b8uikuyesgteslaodM";
    public const string LoginIv = "6P81acb8b8uikuyesgteMasTE";
    public const string ManDays = "ManDays";
    public const string PackingManPower = "PackingManPower";
    public const string UnitsPerHour = "UnitsPerHour";

    public const string IsAvailable = "IsAvailable";

    public const string IndentDate = "IndentDate";
    public const string IndentReplyDate = "IndentReplyDate";
    public const string Indent = "Indent";
    public const string IndentId = "IndentId";
    public const string IndentNo = "IndentNo";
    public const string IndentReply = "IndentReply";
    public const string Eta = "Eta";
    public const string MachineTestId = "MachineTestId";
    public const string MachineTest = "MachineTest";

    public const string IssueToProductionId = "IssueToProductionId";
    public const string IssueToProduction = "IssueToProduction";

    public const string MaterialInward = "MaterialInward";
    public const string Reconcilation = "Reconcilation";

    public const string ProductName = "ProductName";
    public const string WeighingDate = "WeighingDate";

    public const string TestId = "TestId";
    public const string Test = "Test";
    public const string QcId = "QcId";
    public const string Qc = "Qc";
    public const string MinValue = "MinValue";
    public const string MaxValue = "MaxValue";

    public const string PaintingPlan = "PaintingPlan";
    public const string Mto = "Mto";
    public const string CreateMode = "CreateMode";


    public const string OrderQuantity = "OrderQuantity";
    public const string DeliverQuantity = "DeliverQuantity";
    public const string DeliveredQuantity = "DeliveredQuantity";
    public const string Delivered = "Delivered";
    public const string AcceptedQuantity = "AcceptedQuantity";
    public const string PrintedQuantity = "PrintedQuantity";
    public const string PrintQuantity = "PrintQuantity";
    public const string AcceptQuantity = "AcceptQuantity";
    public const string RejectQuantity = "RejectQuantity";
    public const string RackInQuantity = "RackInQuantity";
    public const string RackOutQuantity = "RackOutQuantity";
    public const string SetQuantity = "SetQuantity";
    public const string RackInDate = "RackInDate";

    public const string BatchNo = "BatchNo";
    public const string BatchCard = "BatchCard";
    public const string BatchCardId = "BatchCardId";
    public const string BatchWeight = "BatchWeight";
    public const string PieceKeeperId = "PieceKeeperId";
    public const string PieceKeeper = "Piece Keeper";
    public const string BoxFillerId = "BoxFillerId";
    public const string BoxFiller = "Box Filler";
    public const string BoxTaperId = "BoxTaperId";
    public const string BoxTaper = "Box Taper";
    public const string CartonPackerId = "CartonPackerId";
    public const string CartonPacker = "Carton Packer";
    public const string CartonId = "CartonId";
    public const string Carton = "Carton";
    public const string MetalDetectorId = "MetalDetectorId";
    public const string MetalDetector = "Metal Detector";
    

    public const string Takeup = "Takeup";

    public const string XvsY = "X v/s Y";
    public const string XvsT = "X v/s T";
    public const string CycleBased = "Cycle Based";
    public const string GraphType = "GraphType";

    public const string Connected = "Connected";
    public const string ProductSize = "ProductSize";
    public const string MaterialNo = "MaterialNo";
    public const string ModbusIp = "ModbusIp";
    public const string ModbusPort = "ModbusPort";
    public const string ModbusSignal = "ModbusSignal";
    public const string StartBit = "StartBit";
    public const string CompletedBit = "CompletedBit";
    public const string TargetCycleRegister = "TargetCycleRegister";
    public const string PausedBit = "PausedBit";
    public const string ActualCycleRegister = "ActualCycleRegister";
    public const string NextPauseRegister = "NextPauseRegister";

    public const string Pass = "Pass";
    public const string Fail = "Fail";
    public const string GetMenus = "GetMenus";
    public const string Login = "Login";
    public const string GetPackingTypes = "GetPackingTypes";

    public const string ToolId = "ToolId";
    public const string Tool = "Tool";
    public const string ToolName = "ToolName";
    public const string SheetTypeId = "SheetTypeId";
    public const string SheetType = "SheetType";
    public const string ToolMonitoringId = "ToolMonitoringId";
    public const string ToolMonitoring = "ToolMonitoring";

    public const string PackingTypeId = "PackingTypeId";
    public const string PackingTypeIds = "PackingTypeIds";
    public const string PackingType = "PackingType";
    public const string PackingTypes = "PackingTypes";
    public const string PackingTypeId1 = "PackingTypeId1";
    public const string PackingType1 = "PackingType1";
    public const string Nos = "Nos";

    // University
    public const string InstanceId = "InstanceId";
    public const string InstanceName = "InstanceName";
    public const string FacultyId = "FacultyId";
    public const string Faculty = "Faculty";
    public const string CollegeId = "CollegeId";
    public const string College = "College";
    public const string CollegeName = "CollegeName";
    public const string CourseId = "CourseId";
    public const string Course = "Course";
    public const string CoursePartId = "CoursePartId";
    public const string CoursePartName = "CoursePartName";
    public const string CoursePart = "CoursePart";
    public const string BranchId = "BranchId";
    public const string Branch = "Branch";
    public const string BranchLabel = "BranchLabel";
    public const string PrnNo = "PrnNo";
    public const string StaffId = "StaffId";
    public const string Staff = "Staff";
    public const string StaffName = "StaffName";
    public const string SubjectId = "SubjectId";
    public const string SubjectName = "SubjectName";
    public const string Subject = "Subject";
    public const string SubjectCategoryId = "SubjectCategoryId";
    public const string SubjectCategory = "SubjectCategory";
    public const string RootCategoryId = "RootCategoryId";
    public const string BosId = "BosId";
    public const string Bos = "Bos";
    public const string BosStaffId = "BosStaffId";
    public const string BosStaff = "BosStaff";
    public const string DesignationId1 = "DesignationId1";
    public const string Designation1 = "Designation1";
    public const string DesignationId2 = "DesignationId2";
    public const string Designation2 = "Designation2";
    public const string ShortName = "ShortName";
    public const string Marks = "Marks";
    public const string MaxMarks = "MaxMarks";
    public const string PassingMarks = "PassingMarks";
    public const string ExemptionMarks = "ExemptionMarks";
    public const string SetsAvailable = "SetsAvailable";
    public const string StandardSets = "StandardSets";
    public const string BalanceSets = "BalanceSets";
    public const string MinGrade = "MinGrade";
    public const string CommonSubjectId = "CommonSubjectId";
    public const string Narration1 = "Narration1";
    public const string Narration2 = "Narration2";
    public const string SubjectCategoryMaps = "SubjectCategoryMaps";
    public const string Medical = "Medical";
    public const string OnlyInternal = "OnlyInternal";
    public const string Theory = "THEORY";
    public const string Appointed = "Appointed";
    public const string UnAppointed = "UnAppointed";
    public const string AppointmentType = "AppointmentType";
    public const string IsBarred = "IsBarred";
    public const string IsInternal = "IsInternal";
    public const string IsChairman = "IsChairman";
    public const string IsPaperSetter = "IsPaperSetter";
    public const string IsModerator = "IsModerator";
    public const string IsManuscript = "IsManuscript";
    public const string Original = "Original";
    public const string NoOfAttempts = "NoOfAttempts";
    public const string OutwardNo = "OutwardNo";
    public const string MeetingDate = "MeetingDate";
    public const string MeetingTime = "MeetingTime";
    public const string Email = "Email";
    public const string EmailGroup = "EmailGroup";
    public const string EmailCount = "EmailCount";
    public const string EmailTrigger = "EmailTrigger";
    public const string EmailTriggerGroup = "EmailTriggerGroup";
    public const string Mobile = "Mobile";
    public const string SmsCount = "SmsCount";
    public const string MinRequirement = "MinRequirement";
    public const string AvailableSets = "AvailableSets";
    public const string SetsToBeDrawn = "SetsToBeDrawn";
    public const string SetsDrawn = "SetsDrawn";
    public const string NoOfStudents = "NoOfStudents";
    public const string PendingToBeDrawn = "PendingToBeDrawn";
    public const string UsedSets = "UsedSets";
    public const string Balance = "Balance";
    public const string ExtraSetsToBeDrawn = "ExtraSetsToBeDrawn";
    public const string CupboardNo = "CupboardNo";
    public const string CoursePartRemuneration = "CoursePartRemuneration";
    public const string RemunerationForOthers = "RemunerationForOthers";

    public const string ExtraQuantity = "ExtraQuantity";
    public const string HeatNo = "HeatNo";

    public const string Layer = "Layer";
    public const string LayerId = "LayerId";
    public const string Photo = "Photo";
    public const string PhotoPath = "PhotoPath";

    public const string MasterData = "MasterData";
    public const string ChildData = "ChildData";
    public const string ImagePath = "ImagePath";
    public const string ImportPath = "ImportPath";
    public const string AppSettings = "AppSettings";

    public const string PortName = "PortName";
    public const string BaudRate = "BaudRate";
    public const string DataBits = "DataBits";
    public const string Parity = "Parity";
    public const string StopBits = "StopBits";
    public const string Handshake = "Handshake";
    public const string DataMode = "DataMode";
    public const string SimulationMode = "SimulationMode";

    public const string Manual = "Manual";
    public const string Auto = "Auto";

    public const string HoldingRegister = "HoldingRegister";

    public const string ChartNo = "ChartNo";
    public const string ChartDate = "ChartDate";
    public const string ChartSubNo = "ChartSubNo";

    public const string Oracle = "Oracle";

    public const string Scan = "Scan";
    public const string Preview = "Preview";
    public const string Print = "Print";
    public const string Submit = "Submit";

    // Plan Types
    public const string All = "All";
    public const string PlanType = "PlanType";
    public const string Home = "Home";
    public const string Desking = "Desking";
    public const string OposService = "Opos Service";
    public const string OposRegular = "Opos Regular";
    public const string OposMockup = "Opos Mockup";
    public const string Genii = "Genii";
    public const string MidRange = "MidRange";
    public const string MidRangeTridentGain = "MidRangeTridentGain";
    public const string OposPlanUpdate = "Opos Plan Update";
    public const string Metal = "Metal";
    public const string MetalCount = "MetalCount";
    public const string Wood = "Wood";
    public const string WoodCount = "WoodCount";

    public const string IoLinkIp = "IoLinkIp";
    public const string MaxLength = "MaxLength";
    public const string MaxWidth = "MaxWidth";
    public const string MaxThickness = "MaxThickness";

    public const string FileName = "FileName";
    public const string FilePath = "FilePath";
    public const string PbFilePath = "PbFilePath";
    public const string PbTextFilePath = "PbTextFilePath";
    public const string OutputFolderPath = "OutputFolderPath";
    public const string XmlPath = "XmlPath";
    public const string BmReportPath = "BmReportPath";
    public const string Path = "Path";

    public const string Sheet1 = "Sheet1";
    public const string Mix1Context = "Mix1Context";

    // Baumer
    public const string OffsetX = "OffsetX";
    public const string OffsetY = "OffsetY";
    public const string PixelFormat = "PixelFormat";
    public const string SerialNumber = "SerialNumber";
    public const string ExposureTime = "ExposureTime";
    public const string TriggerMode = "TriggerMode";
    public const string Gain = "Gain";
    public const string Height = "Height";
    public const string Width = "Width";
    public const string Length = "Length";
    public const string DeviceSerialNo = "DeviceSerialNo";

    public const string Accept = "Accept";

    public const string Length1Port = "Length1Port";
    public const string Length2Port = "Length2Port";
    public const string Width1Port = "Width1Port";
    public const string Width2Port = "Width2Port";
    public const string ThicknessPort = "ThicknessPort";
    public const string ColorPort = "ColorPort";

    public const string FormEnabled = "FormEnabled";

    public const string S1LengthConveyorAddress = "S1LengthConveyorAddress";
    public const string S1LengthStrapMotorAddress = "S1LengthStrapMotorAddress";
    public const string S1LengthStrapAddress = "S1LengthStrapAddress";

    public const string CornoIp = "CornoIp";
    public const string AtsIp = "AtsIp";

    public const string Today = "Today";
    public const string Yesterday = "Yesterday";

    public const string DataSource = "DataSource";

    public const string Cable = "Cable";
    public const string Gland = "Gland";
    public const string CableType = "CableType";
    public const string SealType = "SealType";

    public const string Category = "Category";
    public const string InvtPart = "InvtPart";

    public const string Job = "Job";

    public const string Imos = "Imos";
    public const string ImosGroup = "ImosGroup";
    public const string ImosTrigger = "ImosTrigger";
    public const string ImosTriggerGroup = "ImoslTriggerGroup";


    // Royal Enfield
    public const string S1Length = "S1Length";
    public const string S1Width = "S1Width";
    public const string S2Length = "S2Length";
    public const string S2Width = "S2Width";

    public const string DataTextField = "DataTextField";
    public const string DataValueField = "DataValueField";

    public const string Routing = "Routing";
    public const string Boring = "Boring";
    public const string Boring1B = "Boring1B";
    public const string Boring2B = "Boring2B";
    public const string Routing1R = "Routing1R";
    public const string Routing2R = "Routing2R";
    public const string Cleaning = "Cleaning";

    public const string Report = "Report";
    public const string ReportName = "ReportName";
    public const string Title = "Title";
    public const string Ignore = "Ignore";

    public const string PackedBy = "PackedBy";

    public const string Logout = "Logout";
    public const string About = "About";
    public const string Exit = "Exit";

    public const string Disconnected = "Disconnected";

    public const string CollegeLogin = "CollegeLogin";
    public const string StudentLogin = "StudentLogin";

    public const string Manufacturing = "Manufacturing";
    public const string Bo = "BO";

    public const string Reference = "Reference";
    public const string ContainerNo = "ContainerNo";
    public const string LoadNo = "LoadNo";

    public const string PrintToPrinter = "PrintToPrinter";

    public const string Operation = "Operation";
    public const string OldStatus = "OldStatus";
    public const string NewStatus = "NewStatus";

    public const string StoppageCode = "StoppageCode";
    public const string Interval = "Interval";
    public const string ReceiptNo = "ReceiptNo";
    public const string InspectionNo = "InspectionNo";
    public const string CarcassCode = "CarcassCode";
    public const string ArticleNo = "ArticleNo";
    public const string IsImosLabel = "IsImosLabel";
    public const string RejectLabelCount = "RejectLabelCount";
    public const string ReasonId = "ReasonId";
    public const string InspectionInstruction = "InspectionInstruction";
    public const string InspectionResult = "InspectionResult";
    public const string DispositionId = "DispositionId";
    public const string Result = "Result";

    public const string ShiftInCharge = "ShiftInCharge";
    public const string PdiInCharge = "PdiInCharge";
    public const string MultiplicationFactor = "MultiplicationFactor";

    public const string Select = "Select";

    public const string LayoutDataSource = "LayoutDataSource";
    public const string GridDataSource = "GridDataSource";

    public const string Image = "Image";
    public const string Side = "Side";

    public const string ImportRecords = "ImportRecords";

    public const string AndroidResponse = "AndroidResponse";
    public const string RollNo = "RollNo";

    public const string Reserved = "Reserved";
    public const string Reserved1 = "Reserved1";
    public const string Reserved2 = "Reserved2";
    public const string Reserved3 = "Reserved3";

    public const string ProcessType = "ProcessType";
    public const string Strand = "Strand";
    public const string Direction = "Direction";
    public const string Designation = "Designation";
    public const string Standard = "Standard";
    public const string FinishType = "FinishType";

    public const string ReportType = "ReportType";

    public const string GaId = "GaId";

}