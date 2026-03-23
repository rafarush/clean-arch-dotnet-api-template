# Contributing

Thanks for taking the time to contribute to **clean-arch-dotnet-api-template**!

## How to contribute
1. **Search existing issues/PRs** to avoid duplicates.
2. For **small fixes** (typos, small refactors), feel free to open a PR directly.
3. For **new features / behavioral changes**, please open an issue first to discuss the approach.

## Development setup
### Prerequisites
- .NET SDK (use the version specified by the repo/global.json if present)
- (Optional) Docker / Docker Compose

### Build
```bash
dotnet build
```

### Test
```bash
dotnet test
```

## Commit message convention (required)
This repository uses **Conventional Commits**.

### Format
```
<type>(optional-scope): <description>
```

### Allowed types
- `feat`: new feature
- `fix`: bug fix
- `docs`: documentation only changes
- `style`: formatting, missing semi-colons, etc (no code change)
- `refactor`: code change that neither fixes a bug nor adds a feature
- `perf`: performance improvement
- `test`: adding or updating tests
- `build`: build system or external dependencies
- `ci`: CI configuration changes
- `chore`: other changes that don’t modify src/test code

### Examples
- `feat(api): add health check endpoint`
- `fix(auth): handle expired refresh tokens`
- `docs: update README setup steps`
- `refactor: simplify dependency injection registration`

### Breaking changes
If your change introduces a breaking change, use:
- an exclamation mark: `feat!: change public API surface`
- and/or a footer:
```
BREAKING CHANGE: explain what broke and how to migrate
```

## Pull Request guidelines
- Keep PRs focused and reasonably sized.
- Include tests for bug fixes and new features when applicable.
- Ensure these pass locally before opening a PR:
  - `dotnet build`
  - `dotnet test`

## Code of conduct
Be respectful and constructive in discussions.

## Security issues
If you discover a security vulnerability, please **do not open a public issue**. Report it privately by email.

## Contact
For questions or help, email: rgarciagonzalez59@gmail.com
