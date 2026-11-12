# CentralSIG

## Visao geral

Central de acesso aos modulos instalados do Sistema Integrado de Gerenciamento.

- Localizacao e abertura dos executaveis dos modulos.
- Indicacao de instalacao e acesso as atualizacoes.
- Ponto de entrada para os sistemas departamentais.

## Ambiente de desenvolvimento

- Windows e SDK .NET 10.
- Pacotes e licenca Telerik UI for WPF configurados localmente.
- PostgreSQL com os objetos e permissoes exigidos pelo modulo (nos aplicativos abertos pela Central).
- PowerShell 7 e Inno Setup 6 para gerar instaladores.

Mantenha este repositorio, `BibliotecasSIG` e `Update` como pastas irmas. Os arquivos de projeto usam referencias relativas entre esses repositorios.

O projeto principal e `CentralSIG.csproj`. Abra `CentralSIG.sln` ou execute na raiz do repositorio:

```powershell
dotnet restore CentralSIG.sln
dotnet build CentralSIG.sln -c Release
```

As versoes exatas das dependencias estao no arquivo de projeto. Configure o feed Telerik sem versionar senhas ou tokens.

## Configuracao e cuidados

Confira os caminhos e executaveis dos modulos em `MainWindow.xaml.cs`. A Central abre os aplicativos instalados e nao substitui a instalacao de cada um.

Nao inclua senhas, tokens, arquivos pessoais ou exportacoes reais em commits. Valide consultas e gravacoes em homologacao com uma conta autorizada antes de distribuir.

## Testes e homologacao

Confira abertura das telas, filtros, consultas e fluxos modificados. Integracoes, impressoes e gravacoes precisam de validacao especifica; um build aprovado nao garante acesso ao banco ou aos servicos externos.

## Distribuicao

`deploy.ps1`, `Setup.iss` e `scripts/` implementam a publicacao. O servidor padrao e `https://atualizasig.cipolatti.com.br`.

Mantenha `Version`, `AssemblyVersion` e `FileVersion` iguais no projeto. Para gerar os artefatos sem enviar ao servidor:

```powershell
./deploy.ps1 -Changelog @("Descricao das alteracoes") -SkipServerUpload
```

Para enviar, execute sem `-SkipServerUpload` usando credenciais autorizadas. A publicacao gera um rascunho para homologacao; a liberacao ocorre na Central. Consulte os parametros do script para caminho do Inno Setup e envio de artefatos existentes.

Push Git envia codigo/documentacao; nao equivale a publicar uma versao do aplicativo. Nao versione `bin/`, `obj/`, pacotes ou instaladores gerados.
