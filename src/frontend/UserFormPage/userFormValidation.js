import * as modal from '../shared/modal.js'

export function isUserFormValid(values) {
    let isInt = isAllRankingIntegers(values)

    if (!isInt) {
        modal.showAlert("User Form Rankings must be an integer")
        return false
    }

    let isInRange = isAllIntegersInRange(values)

    if (!isInRange) {
        modal.showAlert("User Form Rankings must be between 1 and 10")
        return false
    }

    let isUnique = isAllRankingUnique(values)

    if (!isUnique) {
        modal.showAlert("All User Form Rankings must be unique")
        return false
    }

    return true
}

// Function to check if all values are integers
function isAllRankingIntegers(values) {
    for (let key in values) {
        if (key == "principal") continue
        if (!Number.isInteger(values[key])) {
            return false;
        }
    }
    return true;
}

function isAllIntegersInRange(values) {
    for (let key in values) {
        if (key == "principal") continue
        if (values[key] < 1 || values[key] > 10) {
            return false;
        }
    }
    return true;
}

// Function to check if all values are unique
function isAllRankingUnique(values) {
    let uniqueValues = new Set();
    for (let key in values) {
        if (key == "principal") continue
        if (uniqueValues.has(values[key])) {
            return false;
        }
        uniqueValues.add(values[key]);
    }
    return true;
}