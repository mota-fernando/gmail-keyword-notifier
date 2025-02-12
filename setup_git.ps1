# Configuração inicial do repositório Git para o Gmail Keyword Notifier

# Defina a URL do seu repositório no GitHub
$gitRepoUrl = "https://github.com/mota-fernando/gmail-keyword-notifier.git"

# Verifica se o Git está instalado
if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Git não encontrado! Instale o Git antes de continuar." -ForegroundColor Red
    exit
}

# Inicializa o repositório Git (caso não esteja inicializado)
if (-not (Test-Path ".git")) {
    Write-Host "🛠️ Inicializando repositório Git..."
    git init
}

# Adiciona o README.md
if (-not (Test-Path "README.md")) {
    Write-Host "📝 Criando README.md..."
    $readmeContent = @"
# Gmail Keyword Notifier 🚀

Este projeto lê emails do Gmail e notifica o usuário na área de trabalho quando um email contém palavras-chave suspeitas.

## 📌 Funcionalidades

- Autenticacao segura via **OAuth 2.0**
- Leitura de emails **nao lidos**
- Filtro de emails baseado em **palavras-chave**
- Notificacao automatica na area de trabalho via **NotifyIcon**

## ▶️ Execução
```sh
dotnet run
"@
$readmeContent | Out-File -FilePath "README.md" -Encoding utf8
}

# Verifica se a branch principal é "master" ou "main"
$branchName = git symbolic-ref --short HEAD
if ($branchName -ne "master" -and $branchName -ne "main") {
    Write-Host "⚠️ A branch atual não é 'master' ou 'main'. Você está em uma branch: $branchName"
    Write-Host "Por favor, altere para a branch principal antes de continuar."
    exit
}

# Verifica se há alterações para serem commitadas
$changes = git status --porcelain
if ($changes) {
    Write-Host "🔄 Alterações detectadas. Realizando commit..."
    git add .
    git commit -m "Configuração inicial do repositório"
} else {
    Write-Host "✅ Nenhuma alteração detectada. Não há necessidade de commit."
}

# Verifica se o repositório remoto está configurado
$remoteExists = git remote -v
if (-not $remoteExists) {
    Write-Host "🔧 Configurando repositório remoto..."
    git remote add origin $gitRepoUrl
} else {
    Write-Host "✅ Repositório remoto já configurado."
}

# Faz o push para o repositório remoto
Write-Host "🚀 Enviando alterações para o repositório remoto..."
git push origin $branchName

Write-Host "✅ Processo finalizado com sucesso!"