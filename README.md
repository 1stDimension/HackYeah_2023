# HackYeah_2023


## Lesson learned

Digital signage of PDF is a complex topic. Ability to create arbitrary
incremental changes after signing the initial document is a challenge.
More details about general issue can be found
(https://pyhanko.readthedocs.io/en/latest/cli-guide/signing.html)[here]. The
issues are solved by using PAdES. However the standard itself has proved
difficult to implement in distributed environment.

Time constraints together with complexity limited our ability to reliably
implement Pdf signing with PAdES. XAdES was planed but turned out to be outside
our ability.

Unfortunately we couldn't find a reliable CAdES implementation usable in Python
or any other of the languages used.

