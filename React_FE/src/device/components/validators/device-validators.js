const minLengthValidator = (value, minLength) => {
    return value.length >= minLength;
};

const requiredValidator = value => {
    return value.trim() !== '';
};

const intValidator = value => {
    const re = /^-?\d+$/; 
    return re.test(value);
};

const validate = (value, rules) => {
    let isValid = true;

    for (let rule in rules) {

        switch (rule) {
            case 'minLength': 
                isValid = isValid && minLengthValidator(value, rules[rule]);
                break;

            case 'isRequired': 
                isValid = isValid && requiredValidator(value);
                break;

            case 'isInt':
                isValid = isValid && intValidator(value);
                break;

            default: 
                isValid = true;
        }

    }

    return isValid;
};

export default validate;
