# Manual funcional (usuarios finales)

## 1. Inicio de sesión
- Accede a la aplicación desde el navegador en el sitio IIS configurado.
- El sistema usa **Windows Authentication**; tu usuario de dominio se valida automáticamente.
- Si no tienes permisos, aparecerá un mensaje de acceso denegado.

## 2. Crear una transferencia
1. Ir a **Crear Transferencia** (`CreateTransfer.aspx`).
2. Seleccionar bodega origen y destino.
3. Elegir artículos por grupo/categoría.
4. Guardar el borrador.
5. Confirmar que el número de documento se haya generado.

## 3. Despachar transferencia
1. Ir a **Transfers** (`Transfers.aspx`).
2. Buscar el borrador por fecha/estado/origen/destino.
3. Abrir el detalle (`TransferDetails.aspx`).
4. Registrar el despacho y confirmar.

## 4. Recibir transferencia
1. Abrir la transferencia despachada.
2. Registrar cantidades recibidas.
3. Confirmar recepción para cerrar el proceso.

## 5. Consultar inventario y bines
- **Product Locator**: búsqueda por código de barras/ítem.
- **WhsItemBin/StoreBines**: consulta de bines por bodega/tienda.
- **sapInventory**: consulta de inventario SAP.

## 6. Reportes
- Acceder a las páginas `R_*.aspx`.
- Aplicar filtros (fechas, bodegas, categorías).
- Descargar/imprimir según la pantalla.

## 7. Preguntas frecuentes y videos
- **PreguntasFrecuentes.aspx**: contiene respuestas operativas.
- **VideosOperativos/Financieros**: módulos de capacitación.

> Para detalles técnicos, ver **[manual-tecnico.md](manual-tecnico.md)**.
