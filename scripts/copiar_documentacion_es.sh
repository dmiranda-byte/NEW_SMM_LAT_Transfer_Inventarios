#!/usr/bin/env bash
set -euo pipefail

DESTINO=${1:-./docs_export}

mkdir -p "${DESTINO}"

# Copiar documentación completa
rsync -a --delete "docs/" "${DESTINO}/docs/"

cat <<'MSG'
Documentación exportada correctamente.
Contenido:
- docs/
MSG
