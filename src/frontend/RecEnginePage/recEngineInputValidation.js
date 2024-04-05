export function validateNumRecs(numrecs) {
    if (numrecs >= 1 && numrecs <= 10) {
        return true;
    } else {
        return false;
    }
}

