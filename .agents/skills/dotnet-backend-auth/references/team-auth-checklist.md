# team-auth-checklist.md

Use this checklist when reviewing authentication/account changes.

1. Is duplicate email/mobile blocked at both validator and service level?
2. Is user activation (`IsActive`) toggled only on successful verification?
3. Is OTP expiry checked with explicit date validation?
4. Are controller methods thin and delegated to service layer?
5. Are JWT responses consistent with current API contract?
