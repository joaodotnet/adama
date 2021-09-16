function getSelectedValues(sel) {
    const results = [];

    for (let i = 0; i < sel.options.length; i++) {
        if (sel.options[i].selected) {
            results[results.length] = sel.options[i].value;
        }
    }
    return results;
};