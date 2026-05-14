$Script:DefaultContract = [ordered]@{
    ApplicationId          = 2
    ApplicationVersion     = 1
    MenuId                 = 10
    RootComponentId        = 2824
    CrudComponentId        = 2825
    ComponentDataSourceId  = 354
    DataSourceId           = 146
    DataSourceName         = "Pantalla_Polizas"
    ModelObject            = "Pantalla_Polizas"
}

$Script:AllowedSearchConfigKeys = @(
    "advanced",
    "allowEmpty",
    "catalog",
    "component",
    "control",
    "dataSource",
    "default",
    "field",
    "format",
    "label",
    "mask",
    "max",
    "min",
    "mode",
    "operator",
    "operators",
    "placeholder",
    "required",
    "type",
    "values",
    "visible"
)

function ConvertTo-PlainHashtable {
    param([object]$Value)

    if ($null -eq $Value) {
        return $null
    }

    if ($Value -is [System.Collections.IDictionary]) {
        $result = [ordered]@{}
        foreach ($key in $Value.Keys) {
            $result[$key] = ConvertTo-PlainHashtable -Value $Value[$key]
        }

        return $result
    }

    if ($Value -is [System.Collections.IEnumerable] -and $Value -isnot [string]) {
        $items = @()
        foreach ($item in $Value) {
            $items += ConvertTo-PlainHashtable -Value $item
        }

        return $items
    }

    if ($Value -is [pscustomobject]) {
        $result = [ordered]@{}
        foreach ($property in $Value.PSObject.Properties) {
            $result[$property.Name] = ConvertTo-PlainHashtable -Value $property.Value
        }

        return $result
    }

    return $Value
}

function Get-ValueByAnyName {
    param(
        [object]$Source,
        [string[]]$Names,
        [object]$Default = $null
    )

    if ($null -eq $Source) {
        return $Default
    }

    $plain = ConvertTo-PlainHashtable -Value $Source
    foreach ($name in $Names) {
        if ($plain -is [System.Collections.IDictionary] -and $plain.Contains($name)) {
            return $plain[$name]
        }
    }

    return $Default
}

function ConvertTo-NullableInt {
    param([object]$Value)

    if ($null -eq $Value -or [string]::IsNullOrWhiteSpace([string]$Value)) {
        return $null
    }

    return [int]$Value
}

function ConvertTo-NullableBool {
    param(
        [object]$Value,
        [bool]$Default = $false
    )

    if ($null -eq $Value -or [string]::IsNullOrWhiteSpace([string]$Value)) {
        return $Default
    }

    if ($Value -is [bool]) {
        return $Value
    }

    $text = ([string]$Value).Trim().ToLowerInvariant()
    return $text -in @("1", "true", "yes", "y", "si")
}

function Test-SensitiveText {
    param([string]$Text)

    if ([string]::IsNullOrWhiteSpace($Text)) {
        return $false
    }

    return $Text -match '(?i)(password|pwd|user\s*id|uid=|connection\s*string|data\s*source|initial\s+catalog|authorization|bearer|token|api[-_ ]?key|secret|private\s*key)'
}

function Test-SqlLikeText {
    param([string]$Text)

    if ([string]::IsNullOrWhiteSpace($Text)) {
        return $false
    }

    return $Text -match '(?is)\bselect\b.+\bfrom\b' `
        -or $Text -match '(?is)\binsert\s+into\b' `
        -or $Text -match '(?is)\bupdate\b.+\bset\b' `
        -or $Text -match '(?is)\bdelete\s+from\b' `
        -or $Text -match '(?is)\bmerge\s+into\b' `
        -or $Text -match '(?is)\bexec(?:ute)?\s+[\w\[\].]+'
}

function Get-TextFingerprint {
    param([string]$Text)

    if ([string]::IsNullOrWhiteSpace($Text)) {
        return $null
    }

    $sha = [System.Security.Cryptography.SHA256]::Create()
    try {
        $bytes = [System.Text.Encoding]::UTF8.GetBytes($Text)
        $hash = $sha.ComputeHash($bytes)
        return ([System.BitConverter]::ToString($hash)).Replace("-", "").ToLowerInvariant()
    }
    finally {
        $sha.Dispose()
    }
}

function Protect-MetadataValue {
    param([object]$Value)

    if ($null -eq $Value) {
        return $null
    }

    if ($Value -is [System.Collections.IDictionary]) {
        $result = [ordered]@{}
        foreach ($key in $Value.Keys) {
            if ([string]$key -match '(?i)(password|secret|token|key|connection)') {
                $result[$key] = "[REDACTED]"
            }
            else {
                $result[$key] = Protect-MetadataValue -Value $Value[$key]
            }
        }

        return $result
    }

    if ($Value -is [System.Collections.IEnumerable] -and $Value -isnot [string]) {
        $items = @()
        foreach ($item in $Value) {
            $items += Protect-MetadataValue -Value $item
        }

        return $items
    }

    $text = [string]$Value
    if (Test-SensitiveText -Text $text) {
        return "[REDACTED]"
    }

    if (Test-SqlLikeText -Text $text) {
        return "[REDACTED_SQL_FRAGMENT]"
    }

    return $Value
}

function New-RedactedSqlEvidence {
    param(
        [object[]]$Values,
        [string]$Source
    )

    $texts = @()
    foreach ($value in $Values) {
        if ($null -ne $value -and -not [string]::IsNullOrWhiteSpace([string]$value)) {
            $texts += [string]$value
        }
    }

    if ($texts.Count -eq 0) {
        return [ordered]@{
            present = $false
            source = $Source
        }
    }

    $joined = $texts -join "`n"
    return [ordered]@{
        present = $true
        source = $Source
        redacted = "[REDACTED_SQL_FRAGMENT]"
        fingerprintSha256 = Get-TextFingerprint -Text $joined
        length = $joined.Length
        note = "SQL heredado solo se conserva como evidencia redaccionada; el extractor no lo ejecuta."
    }
}

function Convert-SearchConfigParams {
    param([object]$Value)

    $warnings = @()
    $unknownKeys = @()
    $parsed = [ordered]@{}

    if ($null -eq $Value -or [string]::IsNullOrWhiteSpace([string]$Value)) {
        return [ordered]@{
            rawPresent = $false
            parsed = $parsed
            unknownKeys = $unknownKeys
            warnings = $warnings
        }
    }

    $rawText = [string]$Value
    $source = $null

    try {
        $source = ConvertFrom-Json -InputObject $rawText -ErrorAction Stop
        $source = ConvertTo-PlainHashtable -Value $source
    }
    catch {
        $source = [ordered]@{}
        foreach ($part in ($rawText -split '[;&]')) {
            if ($part -match '^\s*([^=:\s]+)\s*[=:]\s*(.+?)\s*$') {
                $source[$Matches[1]] = $Matches[2]
            }
        }

        if ($source.Count -eq 0) {
            $warnings += "searchConfigParams no se pudo parsear como JSON ni pares clave-valor."
        }
    }

    foreach ($key in $source.Keys) {
        if ($Script:AllowedSearchConfigKeys -contains $key) {
            $parsed[$key] = Protect-MetadataValue -Value $source[$key]
        }
        else {
            $unknownKeys += $key
        }
    }

    return [ordered]@{
        rawPresent = $true
        parsed = $parsed
        unknownKeys = $unknownKeys
        warnings = $warnings
    }
}

function Assert-PolizasMetadataContract {
    param([hashtable]$Document)

    $expected = $Script:DefaultContract
    $actual = $Document.appBuilder

    foreach ($key in @("applicationId", "applicationVersion", "menuId", "rootComponentId", "crudComponentId", "componentDataSourceId", "dataSourceId")) {
        if ($null -eq $actual[$key]) {
            throw "Metadata obligatoria ausente: appBuilder.$key"
        }
    }

    if ([int]$actual.applicationId -ne $expected.ApplicationId) {
        throw "applicationId inesperado. Esperado $($expected.ApplicationId)."
    }
    if ([int]$actual.menuId -ne $expected.MenuId) {
        throw "menuId inesperado. Esperado $($expected.MenuId)."
    }
    if ([int]$actual.rootComponentId -ne $expected.RootComponentId) {
        throw "rootComponentId inesperado. Esperado $($expected.RootComponentId)."
    }
    if ([int]$actual.crudComponentId -ne $expected.CrudComponentId) {
        throw "crudComponentId inesperado. Esperado $($expected.CrudComponentId)."
    }
    if ([int]$actual.componentDataSourceId -ne $expected.ComponentDataSourceId) {
        throw "componentDataSourceId inesperado. Esperado $($expected.ComponentDataSourceId)."
    }
    if ([int]$actual.dataSourceId -ne $expected.DataSourceId) {
        throw "dataSourceId inesperado. Esperado $($expected.DataSourceId)."
    }

    if ($Document.metadata.fields.Count -lt 1) {
        throw "La salida debe incluir al menos un campo de metadata."
    }
}

function ConvertTo-PolizasMetadataDocument {
    param(
        [hashtable]$RawMetadata,
        [ValidateSet("Fixture", "DryRun", "Live")]
        [string]$Mode
    )

    $plainRaw = ConvertTo-PlainHashtable -Value $RawMetadata
    $application = ConvertTo-PlainHashtable -Value $plainRaw["application"]
    $menu = ConvertTo-PlainHashtable -Value $plainRaw["menu"]
    $components = @(ConvertTo-PlainHashtable -Value $plainRaw["components"])
    $componentDataSource = ConvertTo-PlainHashtable -Value $plainRaw["componentDataSource"]
    $dataSource = ConvertTo-PlainHashtable -Value $plainRaw["dataSource"]
    $fields = @(ConvertTo-PlainHashtable -Value $plainRaw["fields"])
    $dependencies = @(ConvertTo-PlainHashtable -Value $plainRaw["dependencies"])

    $rootComponent = $components | Where-Object { [int](Get-ValueByAnyName -Source $_ -Names @("Id", "id")) -eq $Script:DefaultContract.RootComponentId } | Select-Object -First 1
    $crudComponent = $components | Where-Object { [int](Get-ValueByAnyName -Source $_ -Names @("Id", "id")) -eq $Script:DefaultContract.CrudComponentId } | Select-Object -First 1

    $fieldDocuments = @()
    foreach ($field in $fields) {
        $fieldName = Get-ValueByAnyName -Source $field -Names @("Name", "name", "Field", "field", "FieldName", "fieldName", "DataField", "dataField")
        if ([string]::IsNullOrWhiteSpace([string]$fieldName)) {
            continue
        }

        $label = Get-ValueByAnyName -Source $field -Names @("Label", "label", "Title", "title", "Header", "header") -Default $fieldName
        $searchConfig = Convert-SearchConfigParams -Value (Get-ValueByAnyName -Source $field -Names @("SearchConfigParams", "searchConfigParams"))
        $fieldDocuments += [ordered]@{
            name = Protect-MetadataValue -Value $fieldName
            label = Protect-MetadataValue -Value $label
            dataType = Protect-MetadataValue -Value (Get-ValueByAnyName -Source $field -Names @("DataType", "dataType", "Type", "type"))
            visible = ConvertTo-NullableBool -Value (Get-ValueByAnyName -Source $field -Names @("Visible", "visible", "IsVisible", "isVisible")) -Default $true
            searchable = ConvertTo-NullableBool -Value (Get-ValueByAnyName -Source $field -Names @("Searchable", "searchable", "IsSearchable", "isSearchable")) -Default $searchConfig.rawPresent
            sortable = ConvertTo-NullableBool -Value (Get-ValueByAnyName -Source $field -Names @("Sortable", "sortable", "IsSortable", "isSortable")) -Default $true
            order = ConvertTo-NullableInt -Value (Get-ValueByAnyName -Source $field -Names @("Order", "order", "SortOrder", "sortOrder", "Position", "position"))
            lookup = Protect-MetadataValue -Value (Get-ValueByAnyName -Source $field -Names @("Lookup", "lookup", "Catalog", "catalog"))
            searchConfigParams = $searchConfig
            queryStatic = New-RedactedSqlEvidence -Values @(
                (Get-ValueByAnyName -Source $field -Names @("QueryStatic", "queryStatic", "Sql", "sql"))
            ) -Source "field"
        }
    }

    $fieldDocuments = @($fieldDocuments | Sort-Object -Property @{ Expression = { if ($null -eq $_.order) { 99999 } else { $_.order } } }, name)
    $columns = @($fieldDocuments | Where-Object { $_.visible } | ForEach-Object {
        [ordered]@{
            field = $_.name
            label = $_.label
            order = $_.order
        }
    })
    $filters = @($fieldDocuments | Where-Object { $_.searchable -or $_.searchConfigParams.rawPresent } | ForEach-Object {
        [ordered]@{
            field = $_.name
            label = $_.label
            searchConfig = $_.searchConfigParams.parsed
            requiresReview = ($_.searchConfigParams.unknownKeys.Count -gt 0 -or $_.searchConfigParams.warnings.Count -gt 0)
        }
    })

    $warnings = @()
    foreach ($field in $fieldDocuments) {
        foreach ($warning in $field.searchConfigParams.warnings) {
            $warnings += "$($field.name): $warning"
        }
        foreach ($unknown in $field.searchConfigParams.unknownKeys) {
            $warnings += "$($field.name): searchConfigParams contiene clave no soportada '$unknown'."
        }
    }

    if ($Mode -eq "DryRun") {
        $warnings += "DryRun no conecta con BBDD; solo valida el contrato y la forma de salida."
    }

    $document = [ordered]@{
        schemaVersion = "1.0"
        generatedAtUtc = (Get-Date).ToUniversalTime().ToString("o")
        mode = $Mode
        runtimeContract = $false
        notice = "Artefacto offline para analisis, SDD y scaffolding revisado. No usar como contrato runtime de frontend ni backend."
        appBuilder = [ordered]@{
            applicationId = ConvertTo-NullableInt -Value (Get-ValueByAnyName -Source $application -Names @("Id", "id") -Default $Script:DefaultContract.ApplicationId)
            applicationVersion = ConvertTo-NullableInt -Value (Get-ValueByAnyName -Source $application -Names @("Version", "version") -Default $Script:DefaultContract.ApplicationVersion)
            menuId = ConvertTo-NullableInt -Value (Get-ValueByAnyName -Source $menu -Names @("Id", "id") -Default $Script:DefaultContract.MenuId)
            rootComponentId = ConvertTo-NullableInt -Value (Get-ValueByAnyName -Source $rootComponent -Names @("Id", "id") -Default $Script:DefaultContract.RootComponentId)
            crudComponentId = ConvertTo-NullableInt -Value (Get-ValueByAnyName -Source $crudComponent -Names @("Id", "id") -Default $Script:DefaultContract.CrudComponentId)
            componentDataSourceId = ConvertTo-NullableInt -Value (Get-ValueByAnyName -Source $componentDataSource -Names @("Id", "id") -Default $Script:DefaultContract.ComponentDataSourceId)
            dataSourceId = ConvertTo-NullableInt -Value (Get-ValueByAnyName -Source $dataSource -Names @("Id", "id") -Default $Script:DefaultContract.DataSourceId)
            dataSourceName = Protect-MetadataValue -Value (Get-ValueByAnyName -Source $dataSource -Names @("Name", "name") -Default $Script:DefaultContract.DataSourceName)
            modelObject = Protect-MetadataValue -Value (Get-ValueByAnyName -Source $dataSource -Names @("ModelObject", "modelObject", "ObjectName", "objectName") -Default $Script:DefaultContract.ModelObject)
        }
        traceability = [ordered]@{
            application = [ordered]@{
                id = Get-ValueByAnyName -Source $application -Names @("Id", "id")
                name = Protect-MetadataValue -Value (Get-ValueByAnyName -Source $application -Names @("Name", "name"))
                version = Get-ValueByAnyName -Source $application -Names @("Version", "version")
            }
            menu = [ordered]@{
                id = Get-ValueByAnyName -Source $menu -Names @("Id", "id")
                title = Protect-MetadataValue -Value (Get-ValueByAnyName -Source $menu -Names @("Title", "title", "Name", "name"))
                componentId = Get-ValueByAnyName -Source $menu -Names @("ComponentId", "componentId")
                active = ConvertTo-NullableBool -Value (Get-ValueByAnyName -Source $menu -Names @("Active", "active", "IsActive", "isActive")) -Default $true
            }
            components = @($components | ForEach-Object {
                [ordered]@{
                    id = Get-ValueByAnyName -Source $_ -Names @("Id", "id")
                    name = Protect-MetadataValue -Value (Get-ValueByAnyName -Source $_ -Names @("Name", "name"))
                    type = Protect-MetadataValue -Value (Get-ValueByAnyName -Source $_ -Names @("Type", "type", "ComponentType", "componentType"))
                    category = Protect-MetadataValue -Value (Get-ValueByAnyName -Source $_ -Names @("Category", "category"))
                    parentComponentId = Get-ValueByAnyName -Source $_ -Names @("ParentComponentId", "parentComponentId")
                }
            })
            componentDataSource = [ordered]@{
                id = Get-ValueByAnyName -Source $componentDataSource -Names @("Id", "id")
                componentId = Get-ValueByAnyName -Source $componentDataSource -Names @("ComponentId", "componentId")
                dataSourceId = Get-ValueByAnyName -Source $componentDataSource -Names @("DataSourceId", "dataSourceId")
                name = Protect-MetadataValue -Value (Get-ValueByAnyName -Source $componentDataSource -Names @("Name", "name"))
                queryStatic = New-RedactedSqlEvidence -Values @(
                    (Get-ValueByAnyName -Source $componentDataSource -Names @("QueryStatic", "queryStatic", "Sql", "sql"))
                ) -Source "componentDataSource"
            }
            dataSource = [ordered]@{
                id = Get-ValueByAnyName -Source $dataSource -Names @("Id", "id")
                name = Protect-MetadataValue -Value (Get-ValueByAnyName -Source $dataSource -Names @("Name", "name"))
                type = Protect-MetadataValue -Value (Get-ValueByAnyName -Source $dataSource -Names @("Type", "type", "DataSourceType", "dataSourceType"))
                databaseKind = Protect-MetadataValue -Value (Get-ValueByAnyName -Source $dataSource -Names @("DatabaseKind", "databaseKind", "IdBaseDatos", "idBaseDatos"))
                modelObject = Protect-MetadataValue -Value (Get-ValueByAnyName -Source $dataSource -Names @("ModelObject", "modelObject", "ObjectName", "objectName"))
                queryStatic = New-RedactedSqlEvidence -Values @(
                    (Get-ValueByAnyName -Source $dataSource -Names @("QueryStatic", "queryStatic", "Sql", "sql"))
                ) -Source "dataSource"
            }
            dependencies = Protect-MetadataValue -Value $dependencies
        }
        metadata = [ordered]@{
            fields = $fieldDocuments
            lookups = @($fieldDocuments | Where-Object { $null -ne $_.lookup } | ForEach-Object {
                [ordered]@{
                    field = $_.name
                    lookup = $_.lookup
                    requiresReview = $true
                }
            })
        }
        scaffolding = [ordered]@{
            columns = $columns
            filters = $filters
            labels = [ordered]@{}
            tests = @(
                "Validar que columnas y filtros elegidos se convierten en Vue/TypeScript mantenido como codigo fuente.",
                "Validar que el backend usa whitelist propia y parametros, no metadata IAP_* en runtime.",
                "Validar que QueryStatic queda como evidencia redaccionada y no se ejecuta."
            )
        }
        warnings = $warnings
    }

    foreach ($field in $fieldDocuments) {
        $document.scaffolding.labels[$field.name] = $field.label
    }

    Assert-PolizasMetadataContract -Document $document
    return $document
}

function Get-DryRunMetadata {
    return @{
        application = @{
            Id = $Script:DefaultContract.ApplicationId
            Name = "Aunna Tech"
            Version = $Script:DefaultContract.ApplicationVersion
        }
        menu = @{
            Id = $Script:DefaultContract.MenuId
            Title = "Polizas"
            ComponentId = $Script:DefaultContract.RootComponentId
            Active = $true
        }
        components = @(
            @{
                Id = $Script:DefaultContract.RootComponentId
                Name = "Busqueda Polizas"
                Type = "componentsapp-comp"
                Category = "compcat-seacrh"
            },
            @{
                Id = $Script:DefaultContract.CrudComponentId
                Name = "CrudPoliza"
                Type = "tipocontrol-crudtbl"
                ParentComponentId = $Script:DefaultContract.RootComponentId
            }
        )
        componentDataSource = @{
            Id = $Script:DefaultContract.ComponentDataSourceId
            ComponentId = $Script:DefaultContract.CrudComponentId
            DataSourceId = $Script:DefaultContract.DataSourceId
            Name = "Dat_PantallaPolizas"
        }
        dataSource = @{
            Id = $Script:DefaultContract.DataSourceId
            Name = $Script:DefaultContract.DataSourceName
            Type = "datasourcetype-db"
            DatabaseKind = "tipobd-MO"
            ModelObject = $Script:DefaultContract.ModelObject
        }
        fields = @(
            @{
                Name = "Poliza"
                Label = "Poliza"
                DataType = "string"
                Visible = $true
                Searchable = $true
                Order = 10
                SearchConfigParams = '{"control":"text","operator":"contains","advanced":true}'
            }
        )
        dependencies = @(
            @{
                kind = "component"
                sourceId = $Script:DefaultContract.RootComponentId
                targetId = $Script:DefaultContract.CrudComponentId
                relation = "root-child"
            }
        )
    }
}

function Invoke-WhitelistedSelect {
    param(
        [System.Data.SqlClient.SqlConnection]$Connection,
        [string]$CommandText,
        [hashtable]$Parameters
    )

    $command = $Connection.CreateCommand()
    $command.CommandText = $CommandText
    $command.CommandTimeout = 30

    foreach ($key in $Parameters.Keys) {
        $null = $command.Parameters.AddWithValue($key, $Parameters[$key])
    }

    $adapter = New-Object System.Data.SqlClient.SqlDataAdapter $command
    $table = New-Object System.Data.DataTable
    $null = $adapter.Fill($table)

    $rows = @()
    foreach ($row in $table.Rows) {
        $item = [ordered]@{}
        foreach ($column in $table.Columns) {
            $value = $row[$column.ColumnName]
            if ($value -is [System.DBNull]) {
                $value = $null
            }
            $item[$column.ColumnName] = $value
        }
        $rows += $item
    }

    return $rows
}

function Get-SqlIdentifier {
    param([string]$Name)

    if ([string]::IsNullOrWhiteSpace($Name) -or $Name -notmatch '^[A-Za-z0-9_]+$') {
        throw "ILINIUMTECH__PROGRAM_DATABASE debe ser un identificador SQL simple sin espacios ni separadores."
    }

    return "[$Name]"
}

function Merge-MetadataRows {
    param(
        [object[]]$FieldRows,
        [object[]]$ConfigurationRows
    )

    $fieldById = @{}
    foreach ($field in $FieldRows) {
        $plainField = ConvertTo-PlainHashtable -Value $field
        $id = Get-ValueByAnyName -Source $plainField -Names @("Id", "id")
        if ($null -ne $id) {
            $fieldById[[string]$id] = $plainField
        }
    }

    $merged = @()
    foreach ($configuration in $ConfigurationRows) {
        $plainConfiguration = ConvertTo-PlainHashtable -Value $configuration
        $fieldId = Get-ValueByAnyName -Source $plainConfiguration -Names @("DataSourceFieldId", "dataSourceFieldId", "IdDataSourceField", "idDataSourceField", "IAP_DataSourceFieldId")
        $base = [ordered]@{}

        if ($null -ne $fieldId -and $fieldById.ContainsKey([string]$fieldId)) {
            foreach ($key in $fieldById[[string]$fieldId].Keys) {
                $base[$key] = $fieldById[[string]$fieldId][$key]
            }
        }

        foreach ($key in $plainConfiguration.Keys) {
            $base[$key] = $plainConfiguration[$key]
        }

        $merged += $base
    }

    if ($merged.Count -eq 0) {
        return $FieldRows
    }

    return $merged
}

function Get-LiveMetadata {
    param(
        [int]$ApplicationId,
        [int]$MenuId,
        [int]$RootComponentId,
        [int]$CrudComponentId,
        [int]$ComponentDataSourceId,
        [int]$DataSourceId
    )

    $connectionString = $env:ILINIUMTECH__MASTER_CONNECTION
    $programDatabase = $env:ILINIUMTECH__PROGRAM_DATABASE

    if ([string]::IsNullOrWhiteSpace($connectionString)) {
        throw "Falta ILINIUMTECH__MASTER_CONNECTION. Configurela fuera de Git y vuelva a ejecutar Live."
    }
    if ([string]::IsNullOrWhiteSpace($programDatabase)) {
        throw "Falta ILINIUMTECH__PROGRAM_DATABASE. Ejemplo esperado: AunnaTechADM."
    }

    $db = Get-SqlIdentifier -Name $programDatabase
    $connection = New-Object System.Data.SqlClient.SqlConnection $connectionString

    try {
        $connection.Open()
        $parameters = @{
            "@ApplicationId" = $ApplicationId
            "@MenuId" = $MenuId
            "@RootComponentId" = $RootComponentId
            "@CrudComponentId" = $CrudComponentId
            "@ComponentDataSourceId" = $ComponentDataSourceId
            "@DataSourceId" = $DataSourceId
        }

        $applicationRows = Invoke-WhitelistedSelect -Connection $connection -Parameters $parameters -CommandText "SELECT TOP (1) * FROM $db.dbo.IAP_Application WHERE Id = @ApplicationId"
        $menuRows = Invoke-WhitelistedSelect -Connection $connection -Parameters $parameters -CommandText "SELECT TOP (1) * FROM $db.dbo.IAP_Menu WHERE Id = @MenuId"
        $componentRows = Invoke-WhitelistedSelect -Connection $connection -Parameters $parameters -CommandText "SELECT TOP (20) * FROM $db.dbo.IAP_Component WHERE Id IN (@RootComponentId, @CrudComponentId)"
        $componentDataSourceRows = Invoke-WhitelistedSelect -Connection $connection -Parameters $parameters -CommandText "SELECT TOP (1) * FROM $db.dbo.IAP_ComponentDataSource WHERE Id = @ComponentDataSourceId"
        $dataSourceRows = Invoke-WhitelistedSelect -Connection $connection -Parameters $parameters -CommandText "SELECT TOP (1) * FROM $db.dbo.IAP_DataSource WHERE Id = @DataSourceId"
        $dataSourceFieldRows = Invoke-WhitelistedSelect -Connection $connection -Parameters $parameters -CommandText "SELECT TOP (300) * FROM $db.dbo.IAP_DataSourceField WHERE DataSourceId = @DataSourceId"
        $fieldConfigurationRows = Invoke-WhitelistedSelect -Connection $connection -Parameters $parameters -CommandText "SELECT TOP (300) * FROM $db.dbo.IAP_ComponentDataSourceFieldConfiguration WHERE ComponentDataSourceId = @ComponentDataSourceId"
        $fieldRows = Merge-MetadataRows -FieldRows $dataSourceFieldRows -ConfigurationRows $fieldConfigurationRows

        return @{
            application = $applicationRows | Select-Object -First 1
            menu = $menuRows | Select-Object -First 1
            components = $componentRows
            componentDataSource = $componentDataSourceRows | Select-Object -First 1
            dataSource = $dataSourceRows | Select-Object -First 1
            fields = $fieldRows
            dependencies = @(
                @{
                    kind = "component"
                    sourceId = $RootComponentId
                    targetId = $CrudComponentId
                    relation = "root-child"
                }
            )
        }
    }
    catch {
        throw "No se pudo extraer metadata AppBuilder en Live. Error sanitizado: $($_.Exception.GetType().Name). Revise conectividad, permisos read-only y esquema sin imprimir secretos."
    }
    finally {
        $connection.Dispose()
    }
}

function Invoke-PolizasMetadataExtraction {
    [CmdletBinding()]
    param(
        [ValidateSet("Fixture", "DryRun", "Live")]
        [string]$Mode = "Fixture",

        [string]$FixturePath = (Join-Path $PSScriptRoot "fixtures\polizas-metadata.fixture.json"),

        [string]$OutputPath,

        [int]$ApplicationId = $Script:DefaultContract.ApplicationId,
        [int]$MenuId = $Script:DefaultContract.MenuId,
        [int]$RootComponentId = $Script:DefaultContract.RootComponentId,
        [int]$CrudComponentId = $Script:DefaultContract.CrudComponentId,
        [int]$ComponentDataSourceId = $Script:DefaultContract.ComponentDataSourceId,
        [int]$DataSourceId = $Script:DefaultContract.DataSourceId
    )

    $raw = $null
    if ($Mode -eq "Fixture") {
        if (-not (Test-Path -LiteralPath $FixturePath -PathType Leaf)) {
            throw "Fixture no encontrado: $FixturePath"
        }
        $fixtureObject = Get-Content -Raw -LiteralPath $FixturePath | ConvertFrom-Json
        $raw = ConvertTo-PlainHashtable -Value $fixtureObject
    }
    elseif ($Mode -eq "DryRun") {
        $raw = Get-DryRunMetadata
    }
    else {
        $raw = Get-LiveMetadata `
            -ApplicationId $ApplicationId `
            -MenuId $MenuId `
            -RootComponentId $RootComponentId `
            -CrudComponentId $CrudComponentId `
            -ComponentDataSourceId $ComponentDataSourceId `
            -DataSourceId $DataSourceId
    }

    $document = ConvertTo-PolizasMetadataDocument -RawMetadata $raw -Mode $Mode

    if (-not [string]::IsNullOrWhiteSpace($OutputPath)) {
        $resolvedParent = Split-Path -Parent $OutputPath
        if (-not [string]::IsNullOrWhiteSpace($resolvedParent)) {
            New-Item -ItemType Directory -Force -Path $resolvedParent | Out-Null
        }

        $document | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $OutputPath -Encoding UTF8
    }

    return $document
}

Export-ModuleMember -Function Invoke-PolizasMetadataExtraction, Convert-SearchConfigParams, Protect-MetadataValue, ConvertTo-PolizasMetadataDocument
