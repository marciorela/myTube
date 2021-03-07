# myTube

## Objetivo
Consultar os vídeos do YouTube através de sua api, determinar e liberar somente os vídeos que ainda não foram assistidos com um controle simples e manual.\
A solução é composta por dois projetos principais e alguns projetos auxiliares:
- Módulo de serviço do Windows que faz o monitoramento dos vídeos e faz a inclusão no banco de dados. Como o limite de cota da api é baixo e a busca tem um custo alto, isso é feito apenas 3 vezes por dia.
- Módulo Web (asp.net core)
<br>
<br>

## Características
- Permite um controle um controle para vários usuários, desde que informe os dados de cadastro e sua "ApiKey"
- Permite cadastrar quais canais serão monitorados, informando apenas o código do canal e a data inicial em que os vídeos serão consultados.
- Lista de vídeos não assistidos e controle manual do que já foi assistido, assistir mais tarde, ignorar e favorito.

<br>
<br>

## Em desenvolvimento...
