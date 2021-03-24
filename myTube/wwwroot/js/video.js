function setAssistido(id, nome) {
    let params = {
        url: `/video/assistido/${id}`,
        method: 'get',
        success: () => {
            badgeMinus();
            toastr.success('Video marcado como Assistido', `${nome}`);
            $(`#item-${id}`).fadeOut('slow', () => $(this).remove());
            $(`#item-${id}`).remove();
        },
        error: (resp) => { toastr.error(resp.statusText, `${nome}`); },
        complete: () => { }
    };

    $.ajax(params)
}

function setIgnorado(id, nome) {
    let params = {
        url: `/video/ignorado/${id}`,
        method: 'get',
        success: () => {
            badgeMinus();
            toastr.warning('Video marcado como Ignorado', `${nome}`);
            $(`#item-${id}`).fadeOut('slow', () => $(this).remove());
            $(`#item-${id}`).remove();
        },
        error: (resp) => { toastr.error(resp.statusText, `${nome}`); },
        complete: () => { }
    };

    $.ajax(params)
}

function setFavorito(id, nome) {
    let params = {
        url: `/video/favorito/${id}`,
        method: 'get',
        success: () => {
            badgeMinus();
            toastr.warning('Video marcado como Favorito', `${nome}`);
            $(`#item-${id}`).fadeOut('slow', () => $(this).remove());
            $(`#item-${id}`).remove();
        },
        error: (resp) => { toastr.error(resp.statusText, `${nome}`); },
        complete: () => { }
    };

    $.ajax(params)
}

function setAssistirDepois(id, nome) {
    let params = {
        url: `/video/AssistirDepois/${id}`,
        method: 'get',
        success: () => {
            badgeMinus();
            toastr.info('Video marcado como Assistir Depois', `${nome}`);
            $(`#item-${id}`).fadeOut('slow', () => $(this).remove());
            $(`#item-${id}`).remove();
        },
        error: (resp) => { toastr.error(resp.statusText, `${nome}`); },
        complete: () => { }
    };

    $.ajax(params)
}

function saveProgress(id, progress) {
    let params = {
        url: `/video/progress?id=${id}&progress=${progress}`,
        method: 'get',
        success: () => { },
        error: (resp) => { },
        complete: () => { }
    };

    $.ajax(params)
}

function badgeMinus() {
    document.querySelector('#naoassistido').innerHTML = document.querySelector('#naoassistido').innerHTML - 1;
}

function setConsole(id, nome) {
    console.log('Somente para teste')

    toastr.info('Video NÃO MARCADO', `${nome}`);
    $(`#item-${id}`).fadeOut('slow', () => $(this).remove());
    $(`#item-${id}`).remove();
}
